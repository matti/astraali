# Actual validation

Tested on 26 September 2026 in Chromium 153 on Linux ARM64, using Playwright and SwiftShader software WebGL. Audio was enabled by removing Playwright's default `--mute-audio` argument and activating playback with a real button click. I did not have audible monitoring and **did not listen** to the result.

## Complete run

The production build was played once from start to natural completion after the lifecycle fix, with no seeks, speed changes, loops or further user input. `tests/playback.cjs` is the reproducible test; `tests/playback-report.json` contains timestamped measurements.

- Decoded music and engine duration: **130.40326530612245 s**.
- Elapsed time from the start of monitoring to the returned menu: **130.4461 s**. Monitoring begins just after playback starts; its wall interval is not the engine clock.
- One MP3 request, matching the selected file and SHA-256. Playback rate **1**, loop **false**.
- AudioContext was **running**; **208** sampled intervals had nonzero signal above RMS 0.0001. There are intentional low-energy passages in the supplied music. The analyser branches from the actual music gain node; it does not synthesize or play other sound.
- At engine timer updates, maximum measured difference from the audio source clock was **31.04 ms**. An asynchronous page sample comparing the most recently drawn frame's timer to the current audio clock reached **87.97 ms**, including frame age. This distinction matters on a software renderer.
- The audio source stopped and the launch menu returned automatically. No page errors, shader errors, failed resource responses or external playback dependencies were reported.
- At 1280×720 with Standard selected, median engine frame interval **46.3 ms** (~21.6 fps), 95th percentile **87.8 ms**. This environment is software rendering, not a GPU benchmark. Economy is available for weaker devices; a stable 60 fps on real hardware has not been verified.

The test captured live frames at 2, 8, 25, 40, 49, 60, 78, 96, 106, 112, 116, 125 and 129 seconds. Separate paused frame inspection covered every effect and the type transitions. The opening, return, credits and fade were checked visually. The timing diagram and envelope were independently inspected; musical synchronization was not evaluated by ear.

## Issues found and corrected

- Initial engine import order caused a circular-initialization error. The presentation entry now enters through the engine's own main module.
- The first scene composition hid the foreground type. Named scenes now render through JML scene FBOs and are composited before the engine's text texture layers.
- Camera framing cropped parts of the monument/disassembly. The framing was widened and typography repositioned.
- The first full run finished audio correctly, but the presentation shell observed a canvas that the engine removed on stop. Observing the document lifecycle and handling the absent canvas fixed the returned menu; the uninterrupted full run above was repeated afterward.
- Engine default tool/WebSocket activation was inappropriate for a release player. Two documented default changes disable it.

## Additional targeted checks

A separate smoke test passed at Full quality, verified Escape and replay from time zero, verified the returned menu after a targeted end seek, and checked the 390×844 portrait menu and letterboxed Economy playback. The single-demo selector exposed by the engine on return is hidden by presentation CSS. A final browser check served the release under `/dist/`, loaded and played it at 390×844, returned to the menu, and confirmed no failed requests, external requests or page errors. These are additional checks, not a substitute for the uninterrupted run.

## Remaining limits

There was no listening review and no physical display/speaker latency measurement. The pulse grid is numerically estimated. Chromium was tested; Firefox, Safari/iOS and real GPU performance were not. The two scene shaders use WebGL2 and can be demanding at Full resolution. Vite emits the engine's existing eval warning and a large bundle notice; the tested production bundle runs successfully. The engine evaluates its demo scripts by design, so a host with a restrictive Content Security Policy must allow that engine behavior.
