// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright (c) 2013 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//MIT, 2015, WinterDev
//MIT, 2015-2016, EngineKit, brezza92


#ifndef LIBVROOMJS_H
#define LIBVROOMJS_H 

#include <v8.h>
#include <v8-util.h>
#include <stdlib.h>
#include <stdint.h>
#include <iostream>

using namespace v8;

#define JSOBJECT_MARSHAL_TYPE_DYNAMIC       1
#define JSOBJECT_MARSHAL_TYPE_DICTIONARY    2

// jsvalue (JsValue on the CLR side) is a struct that can be easily marshaled
// by simply blitting its value (being only 16 bytes should be quite fast too).

#define JSVALUE_TYPE_UNKNOWN_ERROR  -1
#define JSVALUE_TYPE_EMPTY			 0
#define JSVALUE_TYPE_NULL            1
#define JSVALUE_TYPE_BOOLEAN         2
#define JSVALUE_TYPE_INTEGER         3
#define JSVALUE_TYPE_NUMBER          4
#define JSVALUE_TYPE_STRING          5
#define JSVALUE_TYPE_DATE            6
#define JSVALUE_TYPE_INDEX           7
#define JSVALUE_TYPE_ARRAY          10
#define JSVALUE_TYPE_STRING_ERROR   11
#define JSVALUE_TYPE_MANAGED        12
#define JSVALUE_TYPE_MANAGED_ERROR  13
#define JSVALUE_TYPE_WRAPPED        14
#define JSVALUE_TYPE_DICT           15
#define JSVALUE_TYPE_ERROR          16
#define JSVALUE_TYPE_FUNCTION       17

#define JSVALUE_TYPE_JSTYPEDEF      18 //my extension
#define JSVALUE_TYPE_INTEGER64      19 //my extension

#ifdef _WIN32 
#define EXPORT __declspec(dllexport)
#else 
#define EXPORT
#endif

#ifdef _WIN32
#include "Windows.h"
#define CALLCONV
#define CALLINGCONVENTION __stdcall 
#define INCREMENT(x) InterlockedIncrement(&x)
#define DECREMENT(x) InterlockedDecrement(&x)
#else 
#define CALLCONV
#define CALLINGCONVENTION
#define INCREMENT(x) __sync_fetch_and_add(&x, 1)
#define DECREMENT(x) __sync_fetch_and_add(&x, -1)
#endif

extern int32_t js_object_marshal_type;

extern long js_mem_debug_engine_count;
extern long js_mem_debug_context_count;
extern long js_mem_debug_managedref_count;
extern long js_mem_debug_script_count;

struct MetCallingArgs;

extern "C" 
{	
	
	typedef void (CALLINGCONVENTION *del_JsBridge)(int mIndex,int methodKind,MetCallingArgs* result);
	//-------------------------------------------------------------------------------------------

    struct jsvalue
    {
        // 8 bytes is the maximum CLR alignment; by putting the union first and a
        // int64_t inside it we make (almost) sure the offset of 'type' will always
        // be 8 and the total size 16. We add a check to JsContext_new anyway.
        
        union 
        {
            int32_t     i32;
            int64_t     i64;
            double      num;
            void       *ptr;
            uint16_t   *str;
            jsvalue    *arr;
        } value;
        
        int32_t         type;
        int32_t         length; // Also used as slot index on the CLR side.
	};
 
	struct jserror
	{
		jsvalue type;
		int32_t line;
		int32_t column;
		jsvalue resource;
		jsvalue message;
		jsvalue exception;
	};
	
	EXPORT void CALLCONV jsvalue_dispose(jsvalue value);
}

class JsEngine;
class JsContext;

// The only way for the C++/V8 side to call into the CLR is to use the function
// pointers (CLR, delegates) defined below.

extern "C" 
{
    // We don't have a keepalive_add_f because that is managed on the managed side.
    // Its definition would be "int (*keepalive_add_f) (ManagedRef obj)".
    
    typedef void (CALLINGCONVENTION *keepalive_remove_f) (int context, int id);
    typedef jsvalue (CALLINGCONVENTION *keepalive_get_property_value_f) (int context, int id, uint16_t* name);
    typedef jsvalue (CALLINGCONVENTION *keepalive_set_property_value_f) (int context, int id, uint16_t* name, jsvalue value);
    typedef jsvalue (CALLINGCONVENTION *keepalive_valueof_f) (int context, int id);
	typedef jsvalue (CALLINGCONVENTION *keepalive_invoke_f) (int context, int id, jsvalue args);
	typedef jsvalue (CALLINGCONVENTION *keepalive_delete_property_f) (int context, int id, uint16_t* name);
	typedef jsvalue (CALLINGCONVENTION *keepalive_enumerate_properties_f) (int context, int id);
}

class JsScript {
public:
	static JsScript *New(JsEngine *engine);
	
	jsvalue Compile(const uint16_t* str, const uint16_t *resourceName);
	void Dispose();
	Persistent<Script> *GetScript() { return script_; }

	inline virtual ~JsScript() {
		DECREMENT(js_mem_debug_script_count);
	}

private:
	inline JsScript() {
		INCREMENT(js_mem_debug_script_count);
	}
	JsEngine *engine_;
	Persistent<Script> *script_;
};

// JsEngine is a single isolated v8 interpreter and is the referenced as an IntPtr
// by the JsEngine on the CLR side.
class JsEngine {
public:
	static JsEngine *New(int32_t max_young_space, int32_t max_old_space);
	static JsEngine *NewFromExistingIsolate(v8::Isolate *isolate);
	//-----------------------------------------------------------------
	void TerminateExecution();

	inline void SetRemoveDelegate(keepalive_remove_f delegate) { keepalive_remove_ = delegate; }
    inline void SetGetPropertyValueDelegate(keepalive_get_property_value_f delegate) { keepalive_get_property_value_ = delegate; }
    inline void SetSetPropertyValueDelegate(keepalive_set_property_value_f delegate) { keepalive_set_property_value_ = delegate; }
    inline void SetValueOfDelegate(keepalive_valueof_f delegate) { keepalive_valueof_ = delegate; }
	inline void SetInvokeDelegate(keepalive_invoke_f delegate) { keepalive_invoke_ = delegate; }
	inline void SetDeletePropertyDelegate(keepalive_delete_property_f delegate) { keepalive_delete_property_ = delegate; }
	inline void SetEnumeratePropertiesDelegate(keepalive_enumerate_properties_f delegate) { keepalive_enumerate_properties_ = delegate; }

	// Call delegates into managed code.
    inline void CallRemove(int32_t context, int id) {
		if (keepalive_remove_ == NULL) {
			return;
		}
		keepalive_remove_(context, id); 
	}
    inline jsvalue CallGetPropertyValue(int32_t context, int32_t id, uint16_t* name) {
		if (keepalive_get_property_value_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		jsvalue value = keepalive_get_property_value_(context, id, name);
		return value;
	}
    inline jsvalue CallSetPropertyValue(int32_t context, int32_t id, uint16_t* name, jsvalue value) {
		if (keepalive_set_property_value_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		return keepalive_set_property_value_(context, id, name, value); 
	}
	inline jsvalue CallValueOf(int32_t context, int32_t id) { 
		if (keepalive_valueof_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		return keepalive_valueof_(context, id); 
	}
    inline jsvalue CallInvoke(int32_t context, int32_t id, jsvalue args) { 
		if (keepalive_invoke_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		return keepalive_invoke_(context, id, args); 
	}
	inline jsvalue CallDeleteProperty(int32_t context, int32_t id, uint16_t* name) {
		if (keepalive_delete_property_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		jsvalue value = keepalive_delete_property_(context, id, name);
		return value;
	}
	inline jsvalue CallEnumerateProperties(int32_t context, int32_t id) {
		if (keepalive_enumerate_properties_ == NULL) {
			jsvalue v;
			v.type = JSVALUE_TYPE_NULL;
			return v;
		}
		jsvalue value = keepalive_enumerate_properties_(context, id);
		return value;
	}
	
	// Conversions. Note that all the conversion functions should be called
    // with an HandleScope already on the stack or sill misarabily fail.
    jsvalue ErrorFromV8(TryCatch& trycatch);
    jsvalue StringFromV8(Handle<Value> value);
    jsvalue WrappedFromV8(Handle<Object> obj);
    jsvalue ManagedFromV8(Handle<Object> obj);
    jsvalue AnyFromV8(Handle<Value> value, Handle<Object> thisArg = Handle<Object>());
   
	Persistent<Script> *CompileScript(const uint16_t* str, const uint16_t *resourceName, jsvalue *error);

	// Converts JS function Arguments to an array of jsvalue to call managed code.
    //jsvalue ArrayFromArguments(const Arguments& args);//(0.10.x)
	jsvalue ArrayFromArguments(const FunctionCallbackInfo<Value>& args);//V8(0.12.x)

	Handle<Value> AnyToV8(jsvalue value, int32_t contextId); 
    // Needed to create an array of args on the stack for calling functions.
    int32_t ArrayToV8Args(jsvalue value, int32_t contextId, Handle<Value> preallocatedArgs[]);     

	// Dispose a Persistent<Object> that was pinned on the CLR side by JsObject.
    void DisposeObject(Persistent<Object>* obj);

	void Dispose();
	
	void DumpHeapStats();
	Isolate *GetIsolate() { return isolate_; }

	inline virtual ~JsEngine() {
		DECREMENT(js_mem_debug_engine_count);
	}
	Persistent<Context> *global_context_;

private:
	inline JsEngine() {
		INCREMENT(js_mem_debug_engine_count);
	}

	Isolate *isolate_;
    
	
	Persistent<FunctionTemplate> *managed_template_;
	Persistent<FunctionTemplate> *valueof_function_template_;
	
	keepalive_remove_f keepalive_remove_;
    keepalive_get_property_value_f keepalive_get_property_value_;
    keepalive_set_property_value_f keepalive_set_property_value_;
	keepalive_valueof_f keepalive_valueof_;
    keepalive_invoke_f keepalive_invoke_;
	keepalive_delete_property_f keepalive_delete_property_;
	keepalive_enumerate_properties_f keepalive_enumerate_properties_;
};

class ExternalTypeDefinition;
class ManagedRef;

class JsContext {
 public:
    static JsContext* New(int id, JsEngine *engine);
	static JsContext* NewFromExistingContext(int id, JsEngine *engine, Persistent<Context> *context_);
    // Called by bridge to execute JS from managed code.
    jsvalue Execute(const uint16_t* str, const uint16_t *resourceName);  
	jsvalue Execute(JsScript *script);  

	jsvalue GetGlobal();
    jsvalue GetVariable(const uint16_t* name);
    jsvalue SetVariable(const uint16_t* name, jsvalue value);
	jsvalue GetPropertyNames(Persistent<Object>* obj);
    jsvalue GetPropertyValue(Persistent<Object>* obj, const uint16_t* name);
    jsvalue SetPropertyValue(Persistent<Object>* obj, const uint16_t* name, jsvalue value);
    jsvalue InvokeProperty(Persistent<Object>* obj, const uint16_t* name, jsvalue args);
    jsvalue InvokeFunction(Persistent<Function>* func, Persistent<Object>* thisArg, jsvalue args);
    void Dispose();

	 
	ExternalTypeDefinition* RegisterTypeDefinition(int mIndex,const char* stream,int streamLength);
	void RegisterManagedCallback(void* callback,int callBackKind);	
	ManagedRef* CreateWrapperForManagedObject(int mIndex, ExternalTypeDefinition* externalTypeDef);

	jsvalue ConvAnyFromV8(Handle<Value> value, Handle<Object> thisArg);
	Handle<Value> AnyToV8(jsvalue v);

	inline int32_t GetId() {
		return id_;
	}

	inline virtual ~JsContext() {
		DECREMENT(js_mem_debug_context_count);
	}


	
	del_JsBridge myMangedCallBack;
 private:             
    inline JsContext() {
		INCREMENT(js_mem_debug_context_count);
	}

	int32_t id_;
    Isolate *isolate_;
	JsEngine *engine_;
	Persistent<Context> *context_;
	
	
}; 

class ManagedRef {
 public:

    
    inline explicit ManagedRef(JsEngine *engine, int32_t contextId, int id, bool isJsTypeDef) :
		engine_(engine), 
		contextId_(contextId), 
		id_(id),
		isJsTypeDef_(isJsTypeDef)
	{	

		INCREMENT(js_mem_debug_managedref_count);
	}
    inline int32_t Id() { return id_; }
    inline bool IsJsTypeDef() { return isJsTypeDef_; }

    Handle<Value> GetPropertyValue(Local<String> name);
    Handle<Value> SetPropertyValue(Local<String> name, Local<Value> value);
	Handle<Value> GetValueOf();
    //Handle<Value> Invoke(const Arguments& args);//(0.10.x)
	Handle<Value> Invoke(const FunctionCallbackInfo<Value>& args);//(0.12.x)
    Handle<Boolean> DeleteProperty(Local<String> name);
	Handle<Array> EnumerateProperties();

	v8::Persistent<v8::Object> v8InstanceHandler;
	

    ~ManagedRef() 
	{ 
		engine_->CallRemove(contextId_, id_); 
		DECREMENT(js_mem_debug_managedref_count);
	}
    
 private:
    ManagedRef()
	{
		INCREMENT(js_mem_debug_managedref_count);
	}
	int32_t contextId_;
	JsEngine *engine_;
	int32_t id_;
	bool isJsTypeDef_;
};



class CallingContext
{
public:
	int mIndex;
	JsContext* ctx;
};



class BinaryStreamReader
{

public:
	const char* stream;
	int length;
	int pos;

	BinaryStreamReader(const char* stream,int length);
	int ReadInt16();
	int ReadInt32();
	std::wstring ReadUtf16String();
};

class ExternalTypeDefinition 
{

public:		
	int managedIndex; 
	int memberkind;
	Persistent<v8::ObjectTemplate> handlerToJsObjectTemplate;
	ExternalTypeDefinition(int mIndex);
	void ReadTypeDefinitionFromStream(BinaryStreamReader* reader); 
};
 

#endif
