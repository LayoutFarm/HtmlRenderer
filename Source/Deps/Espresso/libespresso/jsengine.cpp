//MIT, 2015-2016, EngineKit, brezza92
#define NODE7

#include <string.h>
#include <iostream>
#include "espresso.h"

long js_mem_debug_engine_count;

extern "C" jsvalue CALLCONV jsvalue_alloc_array(const int32_t length);

static const int Mega = 1024 * 1024;


static void managed_prop_get(Local<String> name, const PropertyCallbackInfo<Value>& info)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_prop_get" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = info.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	info.GetReturnValue().Set(Local<Value>::New(isolate, ref->GetPropertyValue(name)));
}

static void managed_prop_set(Local<String> name, Local<Value> value, const PropertyCallbackInfo<Value>& info)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_prop_set" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = info.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	if (ref == NULL) {
		Local<Value> result;
		info.GetReturnValue().Set(result);
	}
	info.GetReturnValue().Set(Local<Value>::New(isolate, ref->SetPropertyValue(name, value)));
}

static void managed_prop_delete(Local<String> name, const PropertyCallbackInfo<Boolean>& info)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_prop_delete" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = info.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	info.GetReturnValue().Set(Local<Boolean>::New(isolate, ref->DeleteProperty(name)));
}

static void managed_prop_enumerate(const PropertyCallbackInfo<Array>& info)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_prop_enumerate" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = info.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	info.GetReturnValue().Set(Local<Array>::New(isolate, ref->EnumerateProperties()));
}

static void managed_call(const FunctionCallbackInfo<v8::Value>& args)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_call" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = args.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	args.GetReturnValue().Set(scope.Escape(Local<Value>::New(isolate, ref->Invoke(args))));

}

void managed_valueof(const FunctionCallbackInfo<Value>& args) {
#ifdef DEBUG_TRACE_API
	std::cout << "managed_valueof" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	EscapableHandleScope scope(isolate);

	Local<Object> self = args.Holder();
	Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	args.GetReturnValue().Set(Local<Value>::New(isolate, ref->GetValueOf()));
}

//from nodejs---------------
class ArrayBufferAllocator : public v8::ArrayBuffer::Allocator {
public:
	//ArrayBufferAllocator() :  { }

	//inline void set_env(Environment* env) { env_ = env; }
	// Defined in src/node.cc
	virtual void* Allocate(size_t size);
	virtual void* AllocateUninitialized(size_t size) { return malloc(size); }
	virtual void Free(void* data, size_t) { free(data); }

private:
	//Environment* env_;
};

void* ArrayBufferAllocator::Allocate(size_t size) {
	return calloc(size, 1);
	/*if (env_ == nullptr || !env_->array_buffer_allocator_info()->no_zero_fill())
	return calloc(size, 1);
	env_->array_buffer_allocator_info()->reset_fill_flag();
	return malloc(size);*/
}
//----------------------
//----------------------
JsEngine* JsEngine::NewFromExistingIsolate(v8::Isolate* isolate)
{
	//create engine from existing context
	JsEngine* engine = new JsEngine();
	if (engine == NULL) {
		//eg memory error
		return NULL;
	}
	engine->isolate_ = isolate;
	Locker locker(engine->isolate_);
	Isolate::Scope isolate_scope(engine->isolate_);
	HandleScope scope(engine->isolate_);
	auto context_n = Context::New(engine->isolate_);
	// Setup the template we'll use for all managed object references.
	//FunctionCallback callback;
	Handle<FunctionTemplate> fo = FunctionTemplate::New(engine->isolate_, NULL);
	Handle<ObjectTemplate> obj_template = fo->InstanceTemplate();
	obj_template->SetInternalFieldCount(1);
	obj_template->SetNamedPropertyHandler(
		managed_prop_get,
		managed_prop_set,
		NULL,
		managed_prop_delete,
		managed_prop_enumerate);
	obj_template->SetCallAsFunctionHandler(managed_call);
	engine->managed_template_ = new Persistent<FunctionTemplate>(engine->isolate_, fo);

	Local<FunctionTemplate> ff = FunctionTemplate::New(engine->isolate_, managed_valueof);
	Persistent<FunctionTemplate> fp;
	fp.Reset(engine->isolate_, ff);
	engine->valueof_function_template_ = new Persistent<FunctionTemplate>(engine->isolate_, fp);

	engine->global_context_ = new Persistent<Context>(engine->isolate_, Context::New(engine->isolate_));
	Local<Context> ctx = Local<Context>::New(engine->isolate_, *engine->global_context_);
	ctx->Enter();
	fo->PrototypeTemplate()->Set(String::NewFromUtf8(engine->isolate_, "valueOf"), ff);
	ctx->Exit();

	return engine;
}
//----------------------
JsEngine* JsEngine::New(int32_t max_young_space = -1, int32_t max_old_space = -1)
{
	JsEngine* engine = new JsEngine();
	if (engine == NULL) {
		//eg memory error
		return NULL;
	}
	//---------------------------------------------------
	Isolate::CreateParams params;
	ArrayBufferAllocator* array_buffer_allocator = new ArrayBufferAllocator();
	params.array_buffer_allocator = array_buffer_allocator;
	engine->isolate_ = Isolate::New(params); //
											 //-------------------------------------------------
											 //engine->isolate_->Enter(); 
											 ////if (max_young_space > 0 && max_old_space > 0) {
											 ////	v8::ResourceConstraints constraints;
											 ////	//constraints.set_max_young_space_size(max_young_space * Mega);//0.10.x
											 ////	constraints.set_max_semi_space_size(max_young_space * Mega);//0.12.x
											 ////	constraints.set_max_old_space_size(max_old_space * Mega);
											 ////
											 ////	//v8::SetResourceConstraints(&constraints); //0.10.x
											 //// 	//v8::SetResourceConstraints(engine->isolate_, &constraints); //0.12.x
											 ////	
											 ////	
											 ////} 
											 //engine->isolate_->Exit();
											 //--------------------------------------------------

	Locker locker(engine->isolate_);
	Isolate::Scope isolate_scope(engine->isolate_);
	HandleScope scope(engine->isolate_);
	auto context_n = Context::New(engine->isolate_);
	// Setup the template we'll use for all managed object references.

	Handle<FunctionTemplate> fo = FunctionTemplate::New(engine->isolate_, NULL);
	Handle<ObjectTemplate> obj_template = fo->InstanceTemplate();
	obj_template->SetInternalFieldCount(1);
	obj_template->SetNamedPropertyHandler(
		managed_prop_get,
		managed_prop_set,
		NULL,
		managed_prop_delete,
		managed_prop_enumerate);

	obj_template->SetCallAsFunctionHandler(managed_call);
	engine->managed_template_ = new Persistent<FunctionTemplate>(engine->isolate_, fo);

	Local<FunctionTemplate> ff = FunctionTemplate::New(engine->isolate_, managed_valueof);
	Persistent<FunctionTemplate> fp;
	fp.Reset(engine->isolate_, ff);
	engine->valueof_function_template_ = new Persistent<FunctionTemplate>(engine->isolate_, fp);

	engine->global_context_ = new Persistent<Context>(engine->isolate_, Context::New(engine->isolate_));
	Local<Context> ctx = Local<Context>::New(engine->isolate_, *engine->global_context_);
	ctx->Enter();
	fo->PrototypeTemplate()->Set(String::NewFromUtf8(engine->isolate_, "valueOf"), ff);
	ctx->Exit();

	return engine;
}

Persistent<Script> *JsEngine::CompileScript(const uint16_t* str, const uint16_t *resourceName, jsvalue *error) {
	Locker locker(isolate_);
	Isolate::Scope isolate_scope(isolate_);

	HandleScope scope(isolate_);
	TryCatch trycatch;

	((Context*)global_context_)->Enter();

	Handle<String> source = String::NewFromTwoByte(isolate_, str);
	Handle<Script> script;

	if (resourceName != NULL) {
		Handle<String> name = String::NewFromTwoByte(isolate_, resourceName);

		script = Script::Compile(source, name);
	}
	else {

		script = Script::Compile(source);
	}

	if (script.IsEmpty()) {
		*error = ErrorFromV8(trycatch);
	}

	((Context*)global_context_)->Exit();
	Persistent<Script> *pScript = new Persistent<Script>();
	pScript->Reset(isolate_, script);
	return pScript;
}


void JsEngine::TerminateExecution()
{
	V8::TerminateExecution(isolate_);
}

void JsEngine::DumpHeapStats()
{
	//Locker locker(isolate_);
 //   	Isolate::Scope isolate_scope(isolate_);

	//// gc first.
	//while(!V8::IdleNotification()) {};
	//
	//HeapStatistics stats;
	//isolate_->GetHeapStatistics(&stats);
	//
	//std::wcout << "Heap size limit " << (stats.heap_size_limit() / Mega) << std::endl;
	//std::wcout << "Total heap size " << (stats.total_heap_size() / Mega) << std::endl;
	//std::wcout << "Heap size executable " << (stats.total_heap_size_executable() / Mega) << std::endl;
	//std::wcout << "Total physical size " << (stats.total_physical_size() / Mega) << std::endl;
	//std::wcout << "Used heap size " << (stats.used_heap_size() / Mega) << std::endl;
}

void JsEngine::Dispose()
{
	if (isolate_ != NULL) {
		isolate_->Enter();

		managed_template_->Reset();
		delete managed_template_;
		managed_template_ = NULL;

		valueof_function_template_->Reset();
		delete valueof_function_template_;
		valueof_function_template_ = NULL;

		global_context_->Reset();
		delete global_context_;
		global_context_ = NULL;

		isolate_->Exit();
		isolate_->Dispose();
		isolate_ = NULL;
		keepalive_remove_ = NULL;
		keepalive_get_property_value_ = NULL;
		keepalive_set_property_value_ = NULL;
		keepalive_valueof_ = NULL;
		keepalive_invoke_ = NULL;
		keepalive_delete_property_ = NULL;
		keepalive_enumerate_properties_ = NULL;
	}
}

void JsEngine::DisposeObject(Persistent<Object>* obj)
{
	Locker locker(isolate_);
	Isolate::Scope isolate_scope(isolate_);
	obj->Reset();
}

jsvalue JsEngine::ErrorFromV8(TryCatch& trycatch)
{
	jsvalue v;

	HandleScope scope(isolate_);

	Local<Value> exception = trycatch.Exception();

	v.type = JSVALUE_TYPE_UNKNOWN_ERROR;
	v.value.str = 0;
	v.length = 0;

	// If this is a managed exception we need to place its ID inside the jsvalue
	// and set the type JSVALUE_TYPE_MANAGED_ERROR to make sure the CLR side will
	// throw on it.

	if (exception->IsObject()) {
		Local<Object> obj = Local<Object>::Cast(exception);
		if (obj->InternalFieldCount() == 1) {
			Local<External> wrap = Local<External>::Cast(obj->GetInternalField(0));
			ManagedRef* ref = (ManagedRef*)wrap->Value();
			v.type = JSVALUE_TYPE_MANAGED_ERROR;
			v.length = ref->Id();
			return v;
		}
	}

	jserror *error = new jserror();
	memset(error, 0, sizeof(jserror));

	Local<Message> message = trycatch.Message();

	if (!message.IsEmpty()) {
		error->line = message->GetLineNumber();
		error->column = message->GetStartColumn();
		error->resource = AnyFromV8(message->GetScriptResourceName());
		error->message = AnyFromV8(message->Get());
	}
	if (exception->IsObject()) {
		Local<Object> obj2 = Local<Object>::Cast(exception);
		error->type = AnyFromV8(obj2->GetConstructorName());
	}

	error->exception = AnyFromV8(exception);
	v.type = JSVALUE_TYPE_ERROR;
	v.value.ptr = error;

	return v;
}

jsvalue JsEngine::StringFromV8(Handle<Value> value)
{
	jsvalue v;

	Local<String> s = value->ToString();
	v.length = s->Length();
	v.value.str = new uint16_t[v.length + 1];
	if (v.value.str != NULL) {
		s->Write(v.value.str);
		v.type = JSVALUE_TYPE_STRING;
	}

	return v;
}

jsvalue JsEngine::WrappedFromV8(Handle<Object> obj)
{
	jsvalue v;

	if (js_object_marshal_type == JSOBJECT_MARSHAL_TYPE_DYNAMIC) {
		v.type = JSVALUE_TYPE_WRAPPED;
		v.length = 0;
		// A Persistent<Object> is exactly the size of an IntPtr, right?
		// If not we're in deep deep trouble (on IA32 and AMD64 should be).
		// We should even cast it to void* because C++ doesn't allow to put
		// it in a union: going scary and scarier here.    
		Persistent<Object> *persistent = new Persistent<Object>();
		//persistent->Reset(isolate_,obj);
		v.value.ptr = persistent;//new Persistent<Object>(Persistent<Object>(isolate_, obj));
	}
	else {

		v.type = JSVALUE_TYPE_WRAPPED;
		v.length = 0;
		// A Persistent<Object> is exactly the size of an IntPtr, right?
		// If not we're in deep deep trouble (on IA32 and AMD64 should be).
		// We should even cast it to void* because C++ doesn't allow to put
		// it in a union: going scary and scarier here.    
		//v.value.ptr = new Persistent<Object>(Persistent<Object>::New(obj));
		Persistent<Object> *persistent = new Persistent<Object>();
		persistent->Reset(isolate_, Persistent<Object>(isolate_, obj));
		v.value.ptr = persistent;//new Persistent<Object>(Persistent<Object>(isolate_, obj));

		//-------------------------------------------------------------------------
		/*v.type = JSVALUE_TYPE_DICT;
		Local<Array> names = obj->GetOwnPropertyNames();
		v.length = names->Length();
		jsvalue* values = new jsvalue[v.length * 2];
		if (values != NULL) {
			for(int i = 0; i < v.length; i++) {
				int indx = (i * 2);
				Local<Value> key = names->Get(i);
				values[indx] = AnyFromV8(key);
				values[indx+1] = AnyFromV8(obj->Get(key));
			}
			v.value.arr = values;
		}*/
	}

	return v;
}

jsvalue JsEngine::ManagedFromV8(Handle<Object> obj)
{
	jsvalue v;

	Local<External> wrap = Local<External>::Cast(obj->GetInternalField(0));
	ManagedRef* ref = (ManagedRef*)wrap->Value();

	v.type = ref->IsJsTypeDef() ? JSVALUE_TYPE_JSTYPEDEF : JSVALUE_TYPE_MANAGED;
	v.length = ref->Id();
	v.value.str = 0;

	return v;
}

jsvalue JsEngine::AnyFromV8(Handle<Value> value, Handle<Object> thisArg)
{
	jsvalue v;

	// Initialize to a generic error.
	v.type = JSVALUE_TYPE_UNKNOWN_ERROR;
	v.length = 0;
	v.value.str = 0;

	if (value->IsNull() || value->IsUndefined()) {
		v.type = JSVALUE_TYPE_NULL;
	}
	else if (value->IsBoolean()) {
		v.type = JSVALUE_TYPE_BOOLEAN;
		v.value.i32 = value->BooleanValue() ? 1 : 0;
	}
	else if (value->IsInt32()) {
		v.type = JSVALUE_TYPE_INTEGER;
		v.value.i32 = value->Int32Value();
	}
	else if (value->IsUint32()) {
		v.type = JSVALUE_TYPE_INDEX;
		v.value.i64 = value->Uint32Value();
	}
	else if (value->IsNumber()) {
		v.type = JSVALUE_TYPE_NUMBER;
		v.value.num = value->NumberValue();
	}
	else if (value->IsString()) {
		v = StringFromV8(value);
	}
	else if (value->IsDate()) {
		v.type = JSVALUE_TYPE_DATE;
		//v.value.num = value->NumberValue();		
		v.value.i64 = value->IntegerValue();
	}
	else if (value->IsArray()) {
		Handle<Array> object = Handle<Array>::Cast(value->ToObject());
		v.length = object->Length();
		jsvalue* arr = new jsvalue[v.length];
		if (arr != NULL) {
			for (int i = 0; i < v.length; i++) {
				arr[i] = AnyFromV8(object->Get(i));
			}
			v.type = JSVALUE_TYPE_ARRAY;
			v.value.arr = arr;
		}
	}
	else if (value->IsFunction()) {
		Handle<Function> function = Handle<Function>::Cast(value);
		jsvalue* arr = new jsvalue[2];
		if (arr != NULL) {
			arr[0].value.ptr = new Persistent<Object>(isolate_, Persistent<Function>(isolate_, function));
			arr[0].length = 0;
			arr[0].type = JSVALUE_TYPE_WRAPPED;
			if (!thisArg.IsEmpty()) {
				Persistent<Object> *persistent = new Persistent<Object>();
				persistent->Reset(isolate_, Persistent<Object>(isolate_, thisArg));
				arr[1].value.ptr = persistent;//new Persistent<Object>(Persistent<Object>(isolate_, thisArg));
				arr[1].length = 0;
				arr[1].type = JSVALUE_TYPE_WRAPPED;
			}
			else {
				arr[1].value.ptr = NULL;
				arr[1].length = 0;
				arr[1].type = JSVALUE_TYPE_NULL;
			}
			v.type = JSVALUE_TYPE_FUNCTION;
			v.value.arr = arr;
		}
	}
	else if (value->IsObject()) {
		Handle<Object> obj = Handle<Object>::Cast(value);
		if (obj->InternalFieldCount() == 1)
			v = ManagedFromV8(obj);
		else
			v = WrappedFromV8(obj);
	}

	return v;
}

jsvalue JsEngine::ArrayFromArguments(const FunctionCallbackInfo<Value>& args)
{
	jsvalue v = jsvalue_alloc_array(args.Length());
	Local<Object> thisArg = args.Holder();
	for (int i = 0; i < v.length; i++)
	{
		v.value.arr[i] = AnyFromV8(args[i], thisArg);
	}

	return v;
}


#ifdef NODE7
static void managed_destroy(const v8::WeakCallbackInfo<v8::Local<v8::Object>>& data)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_destroy" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	HandleScope scope(isolate);
	
	void* internalField = data.GetInternalField(0);
	ManagedRef* ref = (ManagedRef*)internalField;
	delete ref;

	//Local<Object> selfHandle1 = Local<Object>::New(isolate, data.GetInternalField(0));//0.12.x 
	//Persistent<Object> self;// = Persistent<Object>(isolate, data.GetValue());//0.12.x 
	//self.Reset(isolate, data);
	//Local<Object> selfHandle = Local<Object>::New(isolate, self);//0.12.x
	//Local<External> wrap = Local<External>::Cast(selfHandle->GetInternalField(0));//0.12.x
	//ManagedRef* ref = (ManagedRef*)wrap->Value();
	//delete ref;
}
#else

static void managed_destroy(const v8::WeakCallbackData<v8::Object, v8::Local<v8::Object>>& data)
{
#ifdef DEBUG_TRACE_API
	std::cout << "managed_destroy" << std::endl;
#endif
	Isolate* isolate = Isolate::GetCurrent();
	HandleScope scope(isolate);

	Persistent<Object> self;// = Persistent<Object>(isolate, data.GetValue());//0.12.x
	self.Reset(isolate, data.GetValue());
	Local<Object> selfHandle = Local<Object>::New(isolate, self);//0.12.x
	Local<External> wrap = Local<External>::Cast(selfHandle->GetInternalField(0));//0.12.x
	ManagedRef* ref = (ManagedRef*)wrap->Value();
	delete ref;
	//puts(NULL);//TODO
	//object.Reset();
	//puts(*data.GetParameter());
	//Isolate* isolate = Isolate::GetCurrent();
 //   HandleScope scope(isolate);
 //   
 //   Persistent<Object> self = Persistent<Object>::Cast(object);

 //   Local<External> wrap = Local<External>::Cast(self->GetInternalField(0));
	//ManagedRef* ref = (ManagedRef*)wrap->Value();
 //   delete ref;
	//object.Reset();
}
#endif

Handle<Value> JsEngine::AnyToV8(jsvalue v, int32_t contextId)
{
	switch (v.type)
	{
	case JSVALUE_TYPE_EMPTY:
		return Handle<Value>();
	case JSVALUE_TYPE_NULL:
		return Null(isolate_);
	case JSVALUE_TYPE_BOOLEAN:
		return Boolean::New(isolate_, v.value.i32 != 0);//TODO
	case JSVALUE_TYPE_INTEGER:
		return Int32::New(isolate_, v.value.i32);
	case JSVALUE_TYPE_NUMBER:
		return Number::New(isolate_, v.value.num);
	case JSVALUE_TYPE_STRING:
		return String::NewFromTwoByte(isolate_, v.value.str);//::New(v.value.str);
	case JSVALUE_TYPE_DATE:
		return Date::New(isolate_, v.value.num);
	case JSVALUE_TYPE_JSTYPEDEF:
	{
		ManagedRef* ext = (ManagedRef*)v.value.ptr;
		Local<Object> obj = Local<Object>::New(isolate_, ext->v8InstanceHandler);

		return obj;
	}
	case JSVALUE_TYPE_ARRAY:
	{ // Arrays are converted to JS native arrays.
		Local<Array> a = Array::New(isolate_, v.length);
		for (int i = 0; i < v.length; i++) {
			a->Set(i, AnyToV8(v.value.arr[i], contextId));
		}
		return a;
	}
	case JSVALUE_TYPE_MANAGED:
	case JSVALUE_TYPE_MANAGED_ERROR:
	{
		// This is an ID to a managed object that lives inside the JsContext keep-alive
		// cache. We just wrap it and the pointer to the engine inside an External. A
		// managed error is still a CLR object so it is wrapped exactly as a normal
		// managed object.
		ManagedRef* ref = new ManagedRef(this, contextId, v.length, false);

		Local<Object> object = ((FunctionTemplate*)managed_template_)->InstanceTemplate()->NewInstance();
		if (object.IsEmpty()) {
			return Null(isolate_);
		}
		object->SetInternalField(0, External::New(isolate_, ref));
		Persistent<Object> persistent(isolate_, object);
#ifdef NODE7
		//persistent.SetWeak(&object, managed_destroy);
		persistent.SetWeak(&object, managed_destroy, v8::WeakCallbackType::kFinalizer);
#else
		persistent.SetWeak(&object, managed_destroy);
#endif
		Local<Object> handle = Local<Object>::New(isolate_, persistent);
		return handle;
	}
	}
	return Null(isolate_);
}

int32_t JsEngine::ArrayToV8Args(jsvalue value, int32_t contextId, Handle<Value> preallocatedArgs[])
{
	if (value.type != JSVALUE_TYPE_ARRAY)
		return -1;

	for (int i = 0; i < value.length; i++) {
		preallocatedArgs[i] = AnyToV8(value.value.arr[i], contextId);
	}

	return value.length;
}
