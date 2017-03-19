//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class NinespaceGrippers
    {
        NinespaceController ninespaceController;
        public NinespaceGrippers(NinespaceController ninespaceController)
        {
            this.ninespaceController = ninespaceController;
        }
        public UIBox LeftGripper
        {
            get;
            set;
        }
        public UIBox TopGripper
        {
            get;
            set;
        }

        public UIBox RightGripper
        {
            get;
            set;
        }
        public UIBox BottomGripper
        {
            get;
            set;
        }


        public void UpdateGripperPositions()
        {
            switch (this.ninespaceController.SpaceConcept)
            {
                default:
                    throw new NotSupportedException();
                case SpaceConcept.NineSpaceFree:
                    return;
                case SpaceConcept.NineSpace:
                case SpaceConcept.FiveSpace:
                    {
                        if (this.LeftGripper != null)
                        {
                            //align on center
                            this.LeftGripper.SetLocation(
                                this.ninespaceController.LeftSpace.Width - (this.LeftGripper.Width / 2),
                                this.ninespaceController.Owner.Height / 2);
                        }
                        if (this.RightGripper != null)
                        {
                            this.RightGripper.SetLocation(
                                this.ninespaceController.RightSpace.X - (this.RightGripper.Width / 2),
                                this.ninespaceController.Owner.Height / 2);
                        }

                        if (this.TopGripper != null)
                        {
                            this.TopGripper.SetLocation(
                               this.ninespaceController.TopSpace.X + (this.ninespaceController.TopSpace.Width / 2) - (this.TopGripper.Width / 2),
                               this.ninespaceController.TopSpace.Bottom - (this.TopGripper.Height / 2));
                        }

                        if (this.BottomGripper != null)
                        {
                            this.BottomGripper.SetLocation(
                               this.ninespaceController.BottomSpace.X + (this.ninespaceController.BottomSpace.Width / 2) - (this.TopGripper.Width / 2),
                               this.ninespaceController.BottomSpace.Y - (this.BottomGripper.Height / 2));
                        }
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                    }
                    break;
                //------------------------------------
                case SpaceConcept.TwoSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical:
                    {
                    }
                    break;
            }
        }
        public void UpdateNinespaces()
        {
            switch (this.ninespaceController.SpaceConcept)
            {
                default:
                    throw new NotSupportedException();
                case SpaceConcept.NineSpaceFree:
                    return;
                case SpaceConcept.NineSpace:
                case SpaceConcept.FiveSpace:
                    {
                        if (this.LeftGripper != null)
                        {
                            //align on center
                            this.ninespaceController.SetLeftSpaceWidth(this.LeftGripper.Left + (this.LeftGripper.Width / 2));
                        }
                        if (this.RightGripper != null)
                        {
                            this.ninespaceController.SetRightSpaceWidth(
                                (this.ninespaceController.Owner.Width - this.RightGripper.Left) - (this.RightGripper.Width / 2));
                        }
                        if (this.TopGripper != null)
                        {
                            this.ninespaceController.SetTopSpaceHeight(this.TopGripper.Top + this.TopGripper.Height / 2);
                        }
                        if (this.BottomGripper != null)
                        {
                            this.ninespaceController.SetBottomSpaceHeight(
                              (this.ninespaceController.Owner.Height - this.BottomGripper.Top) - this.BottomGripper.Height / 2);
                        }
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                    }
                    break;
                //------------------------------------
                case SpaceConcept.TwoSpaceHorizontal:
                    {
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical:
                    {
                    }
                    break;
            }
        }
    }
}