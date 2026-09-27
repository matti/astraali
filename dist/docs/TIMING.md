# Timing score

Sole track: **rock-hybrid**, Kevin MacLeod, unchanged. The supplied duration is 130.403278 s. FFmpeg and Chromium decode **130.40326530612246 s** (a rounding difference of about 13 microseconds). JML's `Effect` sets its end time directly from the decoded track. It does not cut the track at a rounded time.

The supplied onset list and one-second energy estimates were inspected first. `tools/analyze.py` then independently decodes the MP3 at 22,050 Hz, uses a 1,024-sample STFT with 128-sample hops, and measures three bands (35–180, 180–2,200 and 2,200–10,000 Hz). Positive spectral flux is peak-picked. A weighted phase-coherence search between 114 and 118 BPM peaks at **116.000 BPM**, with phase **0.508679 s**, equivalent to **−0.00856247 s** modulo one beat. This refines the publisher tempo with this file's measured phase. It is an estimated grid, not a listening-verified beat/downbeat map.

The generated envelope is sampled every 10 ms and linearly interpolated at the engine time. Bass level affects lighting and body motion; actual spectral energy remains separate from the estimated pulse. Pulse contraction and small editorial marks use that pulse grid. Most major changes happen on deliberately selected phrase-length grid positions. The long reduction after beat 168 suppresses the regular visual pulse, leaving slow disassembly. These are authored choices informed by the spectrum, not automatically inferred musical labels.

| Seconds (approx.) | Image / action |
| --- | --- |
| 0–4.13 | Intake diagram; “this will only take a minute” |
| 4.13–20.68 | Monument and title; camera circles the absurdly honoured object |
| 20.68–37.23 | Multiplication; marching lanes, then overhead view |
| 37.23–45.51 | Printed cone patterns; orientation and density change in groups |
| 45.51–53.78 | Pending decision; reduced blue technical drawing |
| 53.78–70.34 | Cone architecture; continuous forward boulevard motion |
| 70.34–86.89 | Twelve-cone orbital roadblock |
| 86.89–111.72 | Slow disassembly, then gimballed rings; pulse recedes |
| 111.72–113.78 | Thin interruption line during the measured dip/build |
| 113.78–122.06 | Monumental return; “FOREVER”, camera cuts and orbiting barriers |
| 122.06–130.4033 | Receipt/credits; floating remainder and visual fade |

The last visual fade is 0.36 seconds. Audio is never faded, truncated, looped, retimed, reordered or mixed with anything else. The envelope JSON is analysis data, not audio. User activation resumes the same Three.js AudioContext used by JML. JML's `AudioFile`, `Music`, `Timer`, `Loader`, `Player`, scene FBOs and renderer execute the piece; no parallel animation or audio runtime exists.
