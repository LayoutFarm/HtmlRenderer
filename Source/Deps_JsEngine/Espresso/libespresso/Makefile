CXX = g++
CFLAGS = -c -std=c++0x -Wall -fPIC
LDFLAGS = -shared
#v8 header path
V8ROOT = /home/krittamet/ProjectC-CPP/v8_4/v8
HEADERPATH = ${V8ROOT}/include
INCLUDES = -I"${V8ROOT}" -I"${V8ROOT}/include" -I"${V8ROOT}/src" -I"${V8ROOT}/src/base"

SCR = bridge.cpp jscontext.cpp jsengine.cpp jsscript.cpp managedref.cpp
SCR_MINI = bridge2_impl.cpp bridge2.h mini_BinaryReaderWriter.cpp

OBJS = jsscript.o managedref.o mini_BinaryReaderWriter.o bridge.o bridge2_impl.o jscontext.o jsengine.o

#libraries from nodejs
SOLIBPATH = ${V8ROOT}/out/x64.release/lib.target
ALIBPATH = ${V8ROOT}/out/x64.release/obj.target/tools/gyp
LIBPATH = ${SOLIBPATH} ${ALIBPATH}
SHAREDLIB = ${SOLIBPATH}/libv8.so ${SOLIBPATH}/libicui18n.so ${SOLIBPATH}/libicuuc.so
STATICLIB = ${ALIBPATH}/libv8_base.a ${ALIBPATH}/libv8_libbase.a ${ALIBPATH}/libv8_libplatform.a ${ALIBPATH}/libv8_nosnapshot.a
LINKLIB = ${SHAREDLIB} ${STATICLIB}

TARGET = libVRoomJsNative.so

${TARGET}: ${OBJS} vroomjs.h ${STATICLIB}
	${CXX} ${LDFLAGS} ${OBJS} -o ${TARGET} -L${SOLIBPATH} -L${ALIBPATH} ${INCLUDES} -Wl,--whole-archive ${SHAREDLIB} ${STATICLIB} -Wl,--no-whole-archive

vroomjs.h: ${HEADERPATH}/v8.h ${HEADERPATH}/v8-util.h

mini_BinaryReaderWriter.o: mini_BinaryReaderWriter.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} mini_BinaryReaderWriter.cpp

managedref.o: managedref.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} managedref.cpp

jsscript.o: jsscript.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} jsscript.cpp

bridge.o: bridge.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} bridge.cpp

bridge2_impl.o: bridge2_impl.cpp bridge2.h
	${CXX} ${CFLAGS} ${INCLUDES} bridge2_impl.cpp

jscontext.o: jscontext.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} jscontext.cpp

jsengine.o: jsengine.cpp vroomjs.h
	${CXX} ${CFLAGS} ${INCLUDES} jsengine.cpp

clean:
	-rm -f *.o *.a *.so
