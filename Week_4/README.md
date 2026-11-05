# W4 – Physics Engine: Template Project

This is the W4 template project. It is made from the `starter` branch of the [PhysicDemoGame](https://github.com/CADT-Gen11-GameDevelopment/PhysicDemoGame) project and contains only **one scene, the Starter Sandbox**, and the scripts and assets that scene needs. Use it in class to add physics components live in the Inspector and show what they do.

The full project (10 demo scenes, 10 Inspector Labs, main menu and the Physics Courier game) is on the `main` branch of PhysicDemoGame.

## Environment

| Item | Choice |
|---|---|
| Unity version | **6000.3.8f1** (Unity 6.3) |
| Render pipeline | **Built-in Render Pipeline** (Standard shader) |
| Input | **New Input System** (`com.unity.inputsystem` 1.18.0). *Active Input Handling* = Input System Package (New). |
| UI | **UGUI**, built from code with Unity's built-in `LegacyRuntime.ttf` font. TextMeshPro is not used. |
| Physics | Built-in only (PhysX): Rigidbody, Colliders, Joints, Cloth, ParticleSystem. No third-party assets. |

Project settings changed from the defaults:
- Layers 6–10 are `Player`, `Wall`, `Glass`, `Enemy`, `Pickup`.
- Default solver iterations are 10 and velocity iterations are 4, to make joint chains more stable.
- The Game-view gizmo lines use `Assets/Resources/DebugLines.mat` (shader `PhysicsDemo/DebugLines`).

Unity 6 API names (if you compare with older tutorials): `Rigidbody.linearVelocity` (was `velocity`), `linearDamping` / `angularDamping` (was `drag` / `angularDrag`), `PhysicsMaterial` (was `PhysicMaterial`).

## How to open and run

1. In Unity Hub, click **Add → Add project from disk** and select this folder. Open it with **6000.3.8f1**.
2. Open `Assets/Scenes/00_Starter_Sandbox.unity`.
3. Select an object in the Hierarchy, click **Add Component** in the Inspector, then press **Play**.

To get a clean scene again after you saved changes into it, discard them with git:

```bash
git checkout -- Assets/Scenes/00_Starter_Sandbox.unity
```

## Controls

| Input | Action |
|---|---|
| "Camera focus" dropdown | Jump the camera to a station |
| Right mouse drag | Orbit the camera |
| Mouse wheel | Zoom |
| W A S D / Q E | Move the camera / down, up (hold Shift to move faster) |
| T | Slow motion on/off (`Time.timeScale` = 0.2 and `Time.fixedDeltaTime` scaled with it) |
| G | Gizmos on/off (velocity arrows, contact points, joint anchors, collider wireframes) |
| Tab | Show or hide the info panel |
| R / "Reset station (R)" button | Reset only the station chosen in "Camera focus" (all stations when "Overview" is chosen) |
| Shift+R / "Reset scene (Shift+R)" button | Reload the whole scene. The camera and the "Camera focus" choice stay where they were |
| Space | Put the fast balls back at their start and fire them again |

## Stations

Each station is a group in the Hierarchy. The objects have no Rigidbody, joint, Cloth or Particle System yet: these are the components you add.

| Station | Objects | Try |
|---|---|---|
| A · Rigidbody | Cube, Sphere, Capsule floating at 3 m; Platform; Ramp; Wall | Add Rigidbody; change Mass, Use Gravity, Is Kinematic, Linear Damping |
| B · Colliders | Ghost cube (no collider), Rock (mesh, no collider), Table (parent + 5 children, no colliders) | Add a Rigidbody first: the object falls through the floor. Then add a collider. Rock: MeshCollider with Convex on. Table: Rigidbody on the parent, BoxCollider on each child (compound collider) |
| C · Collision detection | Fast ball A and B (with `StartVelocity`, 100 m/s), Thin wall A and B (0.05 m, with `CollisionCounter`), Backstop | Add a Rigidbody to both balls (before or during Play): they fire at the walls, and on Discrete they pass through. Set one ball to Continuous / Continuous Dynamic and press **Space** to put both balls back and fire again. Each wall's label counts its hits and shows the last ball's mode |
| D · Joints | Beam (anchor), Hanging box, Chain (5 links), Door post + Door, Cube A + Cube B | HingeJoint / SpringJoint on the hanging box. Chain: Rigidbody + HingeJoint on each link, Connected Body = the link above. Door: Rigidbody + HingeJoint, Axis (0, 1, 0), Anchor (0, 0, -0.5). Fixed / Spring joint from Cube B to Cube A |
| E · Cloth | Flag (on its pole), Sheet above a ball, a huge sphere collider used as the cloth floor | Add Cloth. Pin the flag's pole edge (Edit Cloth Constraints, or add `ClothPinner`). On the sheet, add the ball and the cloth floor to Cloth ▸ Sphere Colliders (cloth ignores all other colliders) |
| F · Particles | Fire emitter (points up) on a campfire, Spark emitter (points at a wall) | Add Particle System. Try the Shape, Emission, Color over Lifetime and Collision modules |

**Two resets.** *Reset station* (R) puts the focused station's objects back where they started, stops the Rigidbodies, restarts cloth and particles, fires the fast balls again and sets the wall hit counters to 0. Components you added during Play are **kept**, so you can run the same demo again. *Reset scene* (Shift+R) reloads everything, which also removes the components added during Play.

Rule to show in class: changes you make to components **during Play** are undone when Play stops. Changes made outside Play mode are kept when you save the scene.

## Helper components

These scripts in `Assets/Scripts/Common/` can be added with **Add Component** to make a demo easier to see or control:

| Component | What it does |
|---|---|
| `StartVelocity` | Launches the Rigidbody with a set velocity as soon as the object has one; Space puts it back and fires again (used by the fast balls) |
| `CollisionCounter` | Counts `OnCollisionEnter` hits and shows them on the object's `WorldLabel` (used by the thin walls) |
| `VelocityGizmo` | Draws the Rigidbody's velocity as an arrow in the Game view |
| `ContactGizmo` | Shows contact points and normals while two bodies touch |
| `JointGizmo` | Draws each joint's anchor and the line to its connected anchor |
| `ColliderWireframe` | Draws the colliders under an object as wireframes, so the physics shape is visible |
| `ClothPinner` | Pins cloth vertices (for example the flag's pole edge) without painting constraints by hand |
| `MouseGrabber` | Drag Rigidbodies with the mouse (uses a SpringJoint) |
| `KeyboardMover` | Moves a kinematic Rigidbody with the arrow keys (Z / X = down / up) |
| `KinematicMover` | Moves a kinematic Rigidbody back and forth with `MovePosition` / `MoveRotation` |
| `Launcher` | Click to fire a ball from the camera with `AddForce(ForceMode.Impulse)`. Assign a Rigidbody prefab to Projectile Prefab first |
| `ImpactReporter` | Reports the first collision (`OnCollisionEnter`) after it is armed |
| `Coin` | A trigger collectible (`OnTriggerEnter`) that disappears when touched |
| `GuardVision` | A rotating fan of raycasts that detects objects on the Player layer |
| `Lifetime` | Destroys the object after a delay, or when it falls far below the world |

The scene rig (`_DemoRig`) uses `DemoSceneSetup`, `DemoHUD`, `SceneHotkeys` and `StationReset` (the two resets). The camera uses `OrbitCamera` and `CameraFocusMenu`. `Assets/Materials` and `Assets/PhysicMaterials` hold ready-made materials and Physic Materials (Ice, Rubber, Wood, Bounce 0.0 / 0.5 / 0.9...).
