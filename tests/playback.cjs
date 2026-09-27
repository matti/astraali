// End-to-end, real-time playback: no seeking, no speed changes, no mute flag.
const {chromium}=require('playwright');const fs=require('fs');const assert=require('assert');
(async()=>{
 const browser=await chromium.launch({headless:true,ignoreDefaultArgs:['--mute-audio'],args:['--no-sandbox','--enable-unsafe-swiftshader']});
 const page=await browser.newPage({viewport:{width:1280,height:720}});const errors=[],requests=[];
 page.on('pageerror',e=>errors.push(e.message));page.on('console',m=>{if(m.type()==='error')errors.push(m.text());});page.on('response',r=>{if(r.status()>=400)errors.push(r.status()+' '+r.url());if(/\.mp3/.test(r.url()))requests.push(r.url());});
 await page.goto(process.env.DEMO_URL||'http://localhost:4173/');await page.waitForFunction(()=>typeof window.startDemo==='function');await page.click('#start');
 await page.waitForFunction(()=>window.demoScore && !DemoEngine.Effect.loading && new DemoEngine.Timer().music.audioFile.audio?.isPlaying,null,{timeout:90000});
 await page.evaluate(()=>{
  const timer=new DemoEngine.Timer(),a=timer.music.audioFile.audio,ctx=a.context,an=ctx.createAnalyser();an.fftSize=2048;a.gain.connect(an);
  const data=new Float32Array(an.fftSize);window.probe={audio:a,timer,context:ctx,analyser:an,wallStart:performance.now(),samples:[],frames:[],last:-1,maxGap:0,sections:demoScore.sections,clockDrift:[]};
  const update=timer.update;timer.update=function(force){update.call(this,force);if(a.isPlaying && this.time>0)probe.clockDrift.push(this.getTimeInSeconds()-(ctx.currentTime-a._startedAt+a.offset));};
  function frame(){let p=probe,t=timer.getTime();if(t>p.last&&p.last>=0){p.frames.push(t-p.last);p.maxGap=Math.max(p.maxGap,t-p.last)}p.last=t;if(document.body.classList.contains('playing'))requestAnimationFrame(frame)}requestAnimationFrame(frame);
  probe.sample=()=>{an.getFloatTimeDomainData(data);const audioTime=ctx.currentTime-a._startedAt+a.offset;const t=timer.getTimeInSeconds();let rms=Math.sqrt(data.reduce((s,v)=>s+v*v,0)/data.length);let row={wall:(performance.now()-probe.wallStart)/1000,timer:t,audioTime,drift:t-audioTime,rms,playing:a.isPlaying,context:ctx.state,rate:a.playbackRate,loop:a.loop};probe.samples.push(row);return row};
 });
 let nextShot=0;const shots=[2,8,25,40,49,60,78,96,106,112,116,125,129];let count=0;
 for(;;){await page.waitForTimeout(500);const row=await page.evaluate(()=>probe.sample());if(count++%20===0)console.log(JSON.stringify(row));
  if(nextShot<shots.length&&row.timer>=shots[nextShot]){await page.screenshot({path:`tests/play-${shots[nextShot]}.png`});nextShot++;}
  const active=await page.evaluate(()=>document.body.classList.contains('playing'));
  if(!active)break;if(row.wall>145)throw Error('Demo did not finish');
 }
 const result=await page.evaluate(()=>({samples:probe.samples,duration:probe.audio.buffer.duration,audioStopped:!probe.audio.isPlaying,rate:probe.audio.playbackRate,loop:probe.audio.loop,context:probe.context.state,wallElapsed:(performance.now()-probe.wallStart)/1000,frames:probe.frames,sections:probe.sections,clockDrift:probe.clockDrift}));
 result.errors=errors;result.audioRequests=requests;result.environment='Chromium 153 / headless / SwiftShader / 1280x720 / Standard / audio enabled';
 const live=result.samples.filter(s=>s.timer>1&&s.playing);result.maxFrameClockDrift=Math.max(...result.clockDrift.map(Math.abs));delete result.clockDrift;result.maxAbsDrift=Math.max(...live.map(s=>Math.abs(s.drift)));result.nonzeroAudioSamples=live.filter(s=>s.rms>.0001).length;
 const frames=result.frames.filter(f=>f>0&&f<10000).sort((a,b)=>a-b);result.medianFrameMs=frames[Math.floor(frames.length/2)];result.p95FrameMs=frames[Math.floor(frames.length*.95)];delete result.frames;
 fs.writeFileSync('tests/playback-report.json',JSON.stringify(result,null,2));
 assert.equal(errors.length,0,errors.join('\n'));assert.equal(requests.length,1);assert.equal(result.rate,1);assert.equal(result.loop,false);assert(result.audioStopped);assert.equal(result.context,'running');assert(result.nonzeroAudioSamples>100);assert(result.maxFrameClockDrift<.05);assert(Math.abs(result.wallElapsed-result.duration)<1.5);
 console.log('PASS',JSON.stringify({duration:result.duration,wall:result.wallElapsed,maxFrameClockDrift:result.maxFrameClockDrift,lastFrameAgeAndDrift:result.maxAbsDrift,medianFrameMs:result.medianFrameMs,p95FrameMs:result.p95FrameMs,nonzeroAudio:result.nonzeroAudioSamples}));await browser.close();
})().catch(e=>{console.error(e);process.exit(1)});
