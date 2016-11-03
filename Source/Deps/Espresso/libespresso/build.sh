#!/bin/sh
g++ jscontext.cpp jsengine.cpp managedref.cpp bridge.cpp -o libVroomJsNative.so -shared -L ~/v8-3.17/out/x64.release/lib.target/ -I ~/v8-3.17/include/ -fPIC -Wl,--no-as-needed -lv8


