module Constants
open Measures
open Utilities

// Rendering dependent parameters
let entitiesRemovalClampX = convertFloat32ToM ( Shared.RenderingData.viewPort.AspectRatio / 2.0f + Shared.RenderingData.viewPort.AspectRatio / 100.0f * 5.0f );
let entitiesRemovalClamp = { X = entitiesRemovalClampX; Y = 0.55f<m> }
let viperClampX = convertFloat32ToM ( Shared.RenderingData.viewPort.AspectRatio / 2.0f - Shared.RenderingData.viewPort.AspectRatio / 100.0f * 5.0f );
let viperPositionClamp = { X = viperClampX; Y = 0.35f<m> }

// World constants
let galacticashields = 100
let gameDuration = 300.0f<s>
let explosionDuration = 1.0f<s>
let pi = 3.14f<rad>

// Cylon ships
let cylonSpeed = 0.4f<m/s>
let cylonShields = 5
let cylonShieldsWarning = 2
let cylonDamage = 5
let cylonProjectilesSpeed = { X = 0.0f<m/s>; Y = 1.0f<m/s> }
let cylonBoundingRadius = convertFloat32ToM Shared.RenderingData.raiderBoundingRadius
let viperBoundingRadius = convertFloat32ToM Shared.RenderingData.viperMarkIIBoundingRadius

// Viper (player ship)
let maxSpeed = 0.4f<m/s>
let maxRollSpeed = 2.0f<rad/s>
let maxRoll = 0.5f<rad>
let cannonShootingTime = 0.1f<s>
let cannonShootHeating = 10.0f<f>
let cannonCooldownTime = 1.0f<s>
let cannonCooldownRate = 20.0f<f/s>
let cannonMaxTemperature = 100.0f<f>
let viperProjectilesSpeed = { X = 0.0f<m/s>; Y = 1.0f<m/s> }
let vipershields = 5