/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
namespace PixelFarm.VectorMath
{
    public class TrackBallController
    {
        public enum MouseDownType { None, Translation, Rotation, Scale };
        private const double Epsilon = 1.0e-5;
        Vector2 screenCenter;
        double rotationTrackingRadius;
        MouseDownType currentTrackingType = MouseDownType.None;
        Matrix4X4 currentRotationMatrix = Matrix4X4.Identity;
        Matrix4X4 currentTranslationMatrix = Matrix4X4.Identity;
        Matrix4X4 localToScreenTransform;
        private Vector3 rotationStart;
        private Vector3 rotationCurrent;
        Quaternion activeRotationQuaternion = Quaternion.Identity;
        Vector2 lastTranslationMousePosition = Vector2.Zero;
        Vector2 lastScaleMousePosition = Vector2.Zero;
        public TrackBallController()
            : this(new Vector2(), 1)
        {
        }

        public TrackBallController(Vector2 screenCenter, double trackBallRadius)
        {
            rotationStart = new Vector3();
            rotationCurrent = new Vector3();
            this.screenCenter = screenCenter;
            this.rotationTrackingRadius = trackBallRadius;
        }

        public TrackBallController(TrackBallController trackBallToCopy)
        {
            this.screenCenter = trackBallToCopy.screenCenter;
            this.rotationTrackingRadius = trackBallToCopy.rotationTrackingRadius;
            this.currentRotationMatrix = trackBallToCopy.currentRotationMatrix;
            this.currentTranslationMatrix = trackBallToCopy.currentTranslationMatrix;
        }

        public double Scale
        {
            get
            {
                Vector3 scaledUnitVector = Vector3.TransformPosition(Vector3.UnitX, this.GetTransform4X4());
                return scaledUnitVector.Length;
            }

            set
            {
                double requiredChange = value / Scale;
                currentTranslationMatrix *= Matrix4X4.CreateScale(requiredChange);
            }
        }

        public void Translate(Vector3 deltaPosition)
        {
            currentTranslationMatrix = Matrix4X4.CreateTranslation(deltaPosition) * currentTranslationMatrix;
        }

        public MouseDownType CurrentTrackingType
        {
            get
            {
                return currentTrackingType;
            }
        }

        private void MapToSphere(Vector2 screenPoint, out Vector3 vector)
        {
            Vector2 deltaFromCenter = screenPoint - screenCenter;
            Vector2 deltaMinus1To1 = deltaFromCenter;
            //Adjust point coords and scale down to range of [-1 ... 1]
            deltaMinus1To1.x = (deltaMinus1To1.x / rotationTrackingRadius);
            deltaMinus1To1.y = (deltaMinus1To1.y / rotationTrackingRadius);
            //Compute square of the length of the vector from this point to the center
            double length = (deltaMinus1To1.x * deltaMinus1To1.x) + (deltaMinus1To1.y * deltaMinus1To1.y);
            //If the point is mapped outside the sphere... (length > radius squared)
            if (length > 1.0)
            {
                //Compute a normalizing factor (radius / sqrt(length))
                double normalizedLength = (1.0 / Math.Sqrt(length));
                //Return the "normalized" vector, a point on the sphere
                vector.x = deltaMinus1To1.x * normalizedLength;
                vector.y = deltaMinus1To1.y * normalizedLength;
                vector.z = 0.0;
            }
            else //Else it's inside
            {
                //Return a vector to a point mapped inside the sphere sqrt(radius squared - length)
                vector.x = deltaMinus1To1.x;
                vector.y = deltaMinus1To1.y;
                vector.z = Math.Sqrt(1.0 - length);
            }

            vector = Vector3.TransformVector(vector, localToScreenTransform);
        }

        //Mouse down
        public virtual void OnMouseDown(Vector2 mousePosition, Matrix4X4 screenToLocal, MouseDownType trackType = MouseDownType.Rotation)
        {
            if (currentTrackingType == MouseDownType.None)
            {
                localToScreenTransform = Matrix4X4.Invert(screenToLocal);
                currentTrackingType = trackType;
                switch (currentTrackingType)
                {
                    case MouseDownType.Rotation:
                        MapToSphere(mousePosition, out rotationStart);
                        break;
                    case MouseDownType.Translation:
                        lastTranslationMousePosition = mousePosition;
                        break;
                    case MouseDownType.Scale:
                        lastScaleMousePosition = mousePosition;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        //Mouse drag, calculate rotation
        public void OnMouseMove(Vector2 mousePosition)
        {
            switch (currentTrackingType)
            {
                case MouseDownType.Rotation:
                    activeRotationQuaternion = Quaternion.Identity;
                    //Map the point to the sphere
                    MapToSphere(mousePosition, out rotationCurrent);
                    //Return the quaternion equivalent to the rotation
                    //Compute the vector perpendicular to the begin and end vectors
                    Vector3 Perp = Vector3.Cross(rotationStart, rotationCurrent);
                    //Compute the length of the perpendicular vector
                    if (Perp.Length > Epsilon)
                    {
                        //if its non-zero
                        //We're ok, so return the perpendicular vector as the transform after all
                        activeRotationQuaternion.X = Perp.x;
                        activeRotationQuaternion.Y = Perp.y;
                        activeRotationQuaternion.Z = Perp.z;
                        //In the quaternion values, w is cosine (theta / 2), where theta is the rotation angle
                        activeRotationQuaternion.W = Vector3.Dot(rotationStart, rotationCurrent);
                    }
                    break;
                case MouseDownType.Translation:
                    {
                        Vector2 mouseDelta = mousePosition - lastTranslationMousePosition;
                        Vector2 scalledDelta = mouseDelta / screenCenter.x * 4.75;
                        Vector3 offset = new Vector3(scalledDelta.x, scalledDelta.y, 0);
                        offset = Vector3.TransformPosition(offset, Matrix4X4.Invert(CurrentRotation));
                        offset = Vector3.TransformPosition(offset, localToScreenTransform);
                        currentTranslationMatrix = currentTranslationMatrix * Matrix4X4.CreateTranslation(offset);
                        lastTranslationMousePosition = mousePosition;
                    }
                    break;
                case MouseDownType.Scale:
                    {
                        Vector2 mouseDelta = mousePosition - lastScaleMousePosition;
                        double zoomDelta = 1;
                        if (mouseDelta.y < 0)
                        {
                            zoomDelta = 1 - (-1 * mouseDelta.y / 100);
                        }
                        else if (mouseDelta.y > 0)
                        {
                            zoomDelta = 1 + (1 * mouseDelta.y / 100);
                        }
                        currentTranslationMatrix *= Matrix4X4.CreateScale(zoomDelta);
                        lastScaleMousePosition = mousePosition;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void OnMouseUp()
        {
            switch (currentTrackingType)
            {
                case MouseDownType.Rotation:
                    currentRotationMatrix = currentRotationMatrix * Matrix4X4.CreateRotation(activeRotationQuaternion);
                    activeRotationQuaternion = Quaternion.Identity;
                    break;
                case MouseDownType.Translation:
                    //currentTranslationMatrix = Matrix4X4.Identity;
                    break;
                case MouseDownType.Scale:
                    break;
                default:
                    throw new NotImplementedException();
            }
            currentTrackingType = MouseDownType.None;
        }

        public void Rotate(Quaternion rotation)
        {
            currentRotationMatrix = currentRotationMatrix * Matrix4X4.CreateRotation(rotation);
        }

        public void OnMouseWheel(int wheelDelta)
        {
            double zoomDelta = 1;
            if (wheelDelta > 0)
            {
                zoomDelta = 1.2;
            }
            else if (wheelDelta < 0)
            {
                zoomDelta = .8;
            }

            currentTranslationMatrix *= Matrix4X4.CreateScale(zoomDelta);
        }

        public Matrix4X4 CurrentRotation
        {
            get
            {
                if (activeRotationQuaternion == Quaternion.Identity)
                {
                    return currentRotationMatrix;
                }

                return currentRotationMatrix * Matrix4X4.CreateRotation(activeRotationQuaternion);
            }
        }

        public Matrix4X4 GetTransform4X4()
        {
            return currentTranslationMatrix * CurrentRotation;
        }

        public Vector2 ScreenCenter
        {
            get
            {
                return screenCenter;
            }

            set
            {
                screenCenter = value;
            }
        }

        public double TrackBallRadius
        {
            get
            {
                return rotationTrackingRadius;
            }

            set
            {
                rotationTrackingRadius = value;
            }
        }
    }
}
