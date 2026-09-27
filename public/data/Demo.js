// TEMPORARY / FOREVER — original demo-specific scene score.
// All scheduling, resource loading, rendering and sound are JML engine operations.
{
 const s=new Settings();s.engine.tool=false;s.tool.server.enabled=false;
 s.engine.preload=true;s.engine.preloadSteps=18;s.engine.enabledLogLevels=['info','warn','error'];
 s.demo.music.musicFile='music.mp3';s.demo.music.spectogramFile=undefined;
 s.demo.music.loop=false;s.demo.music.volume=1;s.demo.timerSpeed=1;
 s.demo.sync.beatsPerMinute=116;s.demo.screen.width=1920;s.demo.screen.height=1080;
 s.demo.renderer.antialias=false;
}
Demo.prototype.init=function(){
 const s=new Settings();const timer=new DemoEngine.Timer();
 // Use the engine's decoded audio duration, avoiding an MP3 padding guess.
 s.demo.duration=undefined;
 let analysis={bands:[[],[],[]]};
 this.loader.addPromise(fetch('data/envelope.json').then(r=>{if(!r.ok)throw Error('Timing data missing');return r.json()}).then(v=>analysis=v));
 const T=()=>timer.getTimeInSeconds();
 const B=n=>Math.max(0,n*60/116-.00856247);
 const beat=()=>Math.max(0,(T()+.00856247)*116/60);
 const band=i=>{const q=T()*100,k=Math.floor(q),f=q-k;const b=analysis.bands[i];return (b[k]||0)*(1-f)+(b[k+1]||0)*f;};
 const pulse=()=>Math.exp(-(beat()%1)*11)*(T()<B(168)||T()>B(220)?1:.14);
 const end=130.4032653;
 const fade=()=>Math.min(1,T()*3,Math.max(0,(end-T())/.36));
 window.demoScore={title:'TEMPORARY / FOREVER',B,beat,band,pulse,duration:end};
 const vars=(a,start)=>[
 {name:'act',value:[a]},{name:'mode',value:[a]},
 {name:'local',value:[()=>T()-start]},
 {name:'bass',value:[()=>band(0)]},{name:'treble',value:[()=>band(2)]},
 {name:'pulse',value:[pulse]},{name:'beat',value:[beat]},{name:'fade',value:[fade]}];
 const sections=[
 ['intake',0,B(8),'print.fs',0],
 ['monument',B(8),B(40),'world.fs',0],
 ['multiplication',B(40),B(72),'world.fs',1],
 ['rubber-stamp',B(72),B(88),'print.fs',1],
 ['pending',B(88),B(104),'world.fs',2],
 ['infrastructure',B(104),B(136),'world.fs',3],
 ['jurisdiction',B(136),B(168),'world.fs',4],
 ['weightless',B(168),B(216),'world.fs',5],
 ['interruption',B(216),B(220),'print.fs',2],
 ['permanent',B(220),B(236),'world.fs',6],
 ['receipt',B(236),end,'world.fs',5]
 ];
 sections.forEach(([name,start,stop,shader,a])=>{
 this.loader.setScene(name);
 this.loader.addAnimation({start:0,duration:stop-start,image:'plane.png',shader:{name:shader,variable:vars(a,start).filter(v=>shader==='world.fs'?v.name!=='mode':!['act','local','treble'].includes(v.name))}});
 });
 this.loader.setScene('main');
 sections.forEach(([name,start,stop])=>this.loader.addAnimation({start,duration:stop-start,layer:1,scene:{name,fbo:{name:name+'-buffer'}}}));
 sections.forEach(([name,start,stop])=>this.loader.addAnimation({start,duration:stop-start,layer:2,image:name+'-buffer.color.fbo'}));
 // Typographic plates are engine textures. Their transforms and fades are engine animations.
 const plate=(name,start,stop,options={})=>{
 const attack=options.attack??.16,release=options.release??.16;
 this.loader.addAnimation({start,duration:stop-start,layer:10,image:'plates/'+name+'.png',
 color:[{a:0},{duration:attack,a:1},{duration:Math.max(0,stop-start-attack-release),a:1},{duration:release,a:0}],
 position:[{x:options.slide?-.03:0,y:0},{duration:attack,x:0}],
 ...(options.extra||{})});
 };
 plate('frame0',0,B(40));plate('minute',.08,B(8),{attack:.1});
 plate('title',B(8),B(28),{slide:true});
 plate('frame1',B(40),B(88));plate('small',B(40),B(48),{slide:true});
 plate('frame2',B(88),B(104));plate('wait',B(88),B(104));
 plate('frame3',B(104),B(136));plate('civil',B(112),B(120),{slide:true});
 plate('frame4',B(136),B(168));plate('road',B(144),B(152),{slide:true});
 plate('frame5',B(168),B(216));plate('nothing',B(176),B(184),{attack:.6,release:.6});plate('solution',B(192),B(204),{attack:.6,release:.6});
 plate('frame6',B(220),B(236));plate('forever',B(220),B(228),{attack:.04,release:.1});
 // Last eight seconds are legible credits, with the object still quietly floating behind them.
 plate('credits',B(236),end-.25,{attack:.2,release:.3});
 // A few precise, restrained editorial stamps rather than a full-frame beat strobe.
 for(const n of [32,34,36,38,76,78,80,82,84,86,232,234,236,238,240,242]){
 this.loader.addAnimation({start:B(n),duration:.16,layer:12,image:'plane.png',scale:{x:.055,y:.012},position:{x:.425,y:-.39},color:[{r:1,g:.24,b:.035,a:.95},{duration:.16,a:0}]});
 }
 // Exposed only as inspection data; no separate rendering loop or audio player.
 window.demoScore.sections=sections.map(([name,start,end])=>({name,start,end}));
};
