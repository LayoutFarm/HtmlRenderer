// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright © 2013 Federico Di Gregorio <fog@initd.org>
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

//MIT, 2015-2016, EngineKit, brezza92

#include <iostream>

#include "espresso.h"

using namespace v8;

long js_mem_debug_managedref_count;

Handle<Value> ManagedRef::GetPropertyValue(Local<String> name)
{
    Handle<Value> res;
    
    String::Value s(name);
    
#ifdef DEBUG_TRACE_API
		std::cout << "GetPropertyValue" << std::endl;
#endif
		Isolate* isolate = Isolate::GetCurrent();
    jsvalue r = engine_->CallGetPropertyValue(contextId_, id_, *s);
	if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_));//0.12.x
        //res = ThrowException(engine_->AnyToV8(r, contextId_));//0.10.x
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::cout << "cleaning up result from getproperty value" << std::endl;
#endif
    // We don't need the jsvalue anymore and the CLR side never reuse them.
    jsvalue_dispose(r);
	return res;
}

Handle<Boolean> ManagedRef::DeleteProperty(Local<String> name)
{
    Handle<Value> res;
    
    String::Value s(name);
    
#ifdef DEBUG_TRACE_API
		std::cout << "DeleteProperty" << std::endl;
#endif
		Isolate* isolate = Isolate::GetCurrent();
    jsvalue r = engine_->CallDeleteProperty(contextId_, id_, *s);
	if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_));//0.12.x
        //res = ThrowException(engine_->AnyToV8(r, contextId_));//0.10.x
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::cout << "cleaning up result from DeleteProperty" << std::endl;
#endif
    // We don't need the jsvalue anymore and the CLR side never reuse them.
    jsvalue_dispose(r);

	return res->ToBoolean();
}

Handle<Value> ManagedRef::SetPropertyValue(Local<String> name, Local<Value> value)
{
    Handle<Value> res;
    
    String::Value s(name);

#ifdef DEBUG_TRACE_API
		std::cout << "SetPropertyValue" << std::endl;
#endif
		Isolate* isolate = Isolate::GetCurrent();
    jsvalue v = engine_->AnyFromV8(value);
    jsvalue r = engine_->CallSetPropertyValue(contextId_, id_, *s, v);
    if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_));//0.12.x
        //res = ThrowException(engine_->AnyToV8(r, contextId_));//0.10.x
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::cout << "cleaning up result from setproperty value" << std::endl;
#endif
    // We don't need the jsvalues anymore and the CLR side never reuse them.
    jsvalue_dispose(v);
    jsvalue_dispose(r);
    
    return res;
}

Handle<Value> ManagedRef::GetValueOf()
{
#ifdef DEBUG_TRACE_API
	std::wcout << "GETTING VALUE OF..........." << std::endl;
#endif
	Handle<Value> res;
	Isolate* isolate = Isolate::GetCurrent();
    jsvalue r = engine_->CallValueOf(contextId_, id_);
    if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_));//0.12.x
        //res = ThrowException(engine_->AnyToV8(r, contextId_));//0.10.x
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::wcout << "cleaning up result from getting value of" << std::endl;
#endif
    // We don't need the jsvalue anymore and the CLR side never reuse them.
	jsvalue_dispose(r);
    
    return res;
}

Handle<Value> ManagedRef::Invoke(const FunctionCallbackInfo<Value>& args)
{
#ifdef DEBUG_TRACE_API
	std::wcout << "INVOKING..........." << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	Handle<Value> res;
    jsvalue a = engine_->ArrayFromArguments(args);
    jsvalue r = engine_->CallInvoke(contextId_, id_, a);
	if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_)); //(0.12.x)
        //res = ThrowException(engine_->AnyToV8(r, contextId_)); //(0.10.x)
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::wcout << "cleaning up result from invoke" << std::endl;
#endif
    // We don't need the jsvalue anymore and the CLR side never reuse them.
	jsvalue_dispose(a);
    jsvalue_dispose(r);
    
    return res;
}

Handle<Array> ManagedRef::EnumerateProperties()
{
    Handle<Value> res;
    
#ifdef DEBUG_TRACE_API
		std::cout << "EnumerateProperties" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
    jsvalue r = engine_->CallEnumerateProperties(contextId_, id_);
	if (r.type == JSVALUE_TYPE_MANAGED_ERROR)
		isolate->ThrowException(engine_->AnyToV8(r, contextId_));//0.12.x
        //res = ThrowException(engine_->AnyToV8(r, contextId_));//0.10.x
    else
        res = engine_->AnyToV8(r, contextId_);
    
#ifdef DEBUG_TRACE_API
		std::cout << "cleaning up result from EnumerateProperties" << std::endl;
#endif
    // We don't need the jsvalue anymore and the CLR side never reuse them.
    jsvalue_dispose(r);

	return Handle<Array>::Cast(res);
}