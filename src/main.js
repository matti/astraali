// Presentation shell only. JML owns all rendering, scene scheduling and audio.
import '../engine/src/main.js';
import * as THREE from 'three';
const button=document.getElementById('start');
button.addEventListener('click',()=>{THREE.AudioContext.getContext().resume();window.startDemo();});
window.addEventListener('keydown',e=>{if(e.key==='Enter')THREE.AudioContext.getContext().resume();});
new MutationObserver(()=>{
 document.body.classList.toggle('playing',document.getElementById('canvas')?.style.display==='block');
}).observe(document.body,{attributes:true,attributeFilter:['style'],childList:true,subtree:true});
