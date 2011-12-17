module Constants
open Measures
open Utilities

// Rendering dependent parameters
let entitiesRemovalClamp = { X = 0.8f<m>; Y = 0.6f<m> }
let viperPositionClamp = { X = 0.8f<m>; Y = 0.45f<m> }

// World constants
let galacticashields = 100
let gameDuration = 300.0f<s>
let explosionDuration = 1.0f<s>
let pi = 3.14f<rad>

// Cylon ships
let cylonSpeed = 0.1f<m/s>
let cylonShields = 7

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
let vipershields = 3