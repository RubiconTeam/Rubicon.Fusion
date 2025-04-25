class_name CameraUpdate

## An enumerator specifying the type of update method on a certain part of the camera.

const NONE : int = 0 ## Doesn't update the camera at all.
const INSTANT : int = 1 ## Immediate setting of the camera's value.
const INTERPOLATION : int = 2 ## Uses interpolation to update the camera over time.
const TWEEN : int = 3 ## Tween the camera's value to the final value.