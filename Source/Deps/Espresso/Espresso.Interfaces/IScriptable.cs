using System;
using System.Collections.Generic;
using System.Text;

namespace VroomJs
{
    public interface INativeRef
    {
        int ManagedIndex { get; }
        object WrapObject { get; }
        bool HasNativeSide { get; }
        void SetUnmanagedPtr(IntPtr unmanagedObjectPtr);
        IntPtr UnmanagedPtr { get; }
    }
    public interface INativeScriptable : INativeRef
    {
        IntPtr UnmanagedTypeDefinitionPtr { get; }
    }

}
