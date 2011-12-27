module Constants
open Measures
open Utilities
open System

// Rendering dependent parameters
let entitiesRemovalClampX = ( Shared.RenderingData.viewPort.AspectRatio / 2.0f + Shared.RenderingData.viewPort.AspectRatio / 100.0f * 5.0f ) * m;
let entitiesRemovalClamp = { X = entitiesRemovalClampX; Y = 0.55f<m> }
let viperClampX = ( Shared.RenderingData.viewPort.AspectRatio / 2.0f - Shared.RenderingData.viewPort.AspectRatio / 100.0f * 5.0f ) * m;
let viperPositionClamp = { X = viperClampX; Y = 0.35f<m> }

// World constants
let galacticashields = 100
let gameDuration = 120.0f<s>
let explosionDuration = 1.0f<s>
let PI = (float32) Math.PI * rad
let minimumSpawnWait = 0.75

// Cylon ships
let cylonSpeed = 0.2f<m/s>
let cylonRollSpeed = 2.0f<rad/s>
let cylonMaxRoll = 0.5f<rad>
let cylonShields = 4
let cylonShieldsWarning = 2
let cylonProjectilesSpeedFactor = 0.65f
let cylonShootingAngle = 0.3f
let cylonBoundingRadius = ( Shared.RenderingData.raiderBoundingRadius * 0.8f ) * m // it's ugly, but must compensate for raider not really spherical shape
let cylonBurstWait = 0.1

// Viper (player ship)
let cannonShootingTime = 0.1f<s>
let cannonShootHeating = 10.0f<f>
let cannonCooldownTime = 1.0f<s>
let cannonCooldownRate = 40.0f<f/s>
let cannonMaxTemperature = 100.0f<f>
let viperProjectilesSpeed = { X = 0.0f<m/s>; Y = 1.0f<m/s> }
let vipershields = 5
let viperBoundingRadius = ( Shared.RenderingData.viperMarkIIBoundingRadius * 0.8f ) * m
let viperSpeed = 0.4f<m/s>
let viperRollSpeed = 2.0f<rad/s>
let viperMaxRoll = 0.5f<rad>
