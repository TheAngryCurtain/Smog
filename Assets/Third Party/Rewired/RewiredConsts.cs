/* Rewired Constants
   This list was generated on 11/6/2017 10:53:55 PM
   The list applies to only the Rewired Input Manager from which it was generated.
   If you use a different Rewired Input Manager, you will have to generate a new list.
   If you make changes to the exported items in the Rewired Input Manager, you will need to regenerate this list.
*/

namespace RewiredConsts {
    public static class Action {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Steer")]
        public const int Steer = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Accelerate")]
        public const int Accelerate = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Brake")]
        public const int Brake = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Drift")]
        public const int Drift = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Camera Horizontal")]
        public const int Camera_Horizontal = 3;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Camera Vertical")]
        public const int Camera_Vertical = 4;
    }
    public static class Category {
        public const int Default = 0;
    }
    public static class Layout {
        public static class Joystick {
            public const int Default = 0;
        }
        public static class Keyboard {
            public const int Default = 0;
        }
        public static class Mouse {
            public const int Default = 0;
        }
        public static class CustomController {
            public const int Default = 0;
        }
    }
}
