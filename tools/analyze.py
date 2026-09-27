import numpy as np, subprocess, json
from scipy import signal
from pathlib import Path
import matplotlib.pyplot as plt
p=Path(__file__).resolve().parents[1];sr=22050
(p/'research').mkdir(exist_ok=True)
raw=subprocess.check_output(['ffmpeg','-v','error','-i',str(p/'public/data/music.mp3'),'-f','f32le','-ac','1','-ar',str(sr),'-'])
x=np.frombuffer(raw,dtype=np.float32)
f,t,z=signal.stft(x,sr,nperseg=1024,noverlap=896); mag=abs(z)
bands=np.array([np.sqrt(np.mean(mag[(f>=lo)&(f<hi)]**2,axis=0)) for lo,hi in [(35,180),(180,2200),(2200,10000)]])
flux=np.maximum(np.diff(mag,axis=1),0).sum(axis=0);ft=t[1:]
peaks,_=signal.find_peaks(flux,distance=int(.20*sr/128),prominence=np.median(flux)*.8)
# Fit a pulse train to flux; this is a numerical estimate, not a listening-verified beat map.
scores=[]
for bpm in np.linspace(114,118,1601):
 phase=ft[peaks]*bpm/60*2*np.pi; scores.append(abs(np.sum(flux[peaks]*np.exp(1j*phase))))
bpm=float(np.linspace(114,118,1601)[np.argmax(scores)]);period=60/bpm
phase=np.angle(np.sum(flux[peaks]*np.exp(2j*np.pi*ft[peaks]/period)))%(2*np.pi);offset=phase/(2*np.pi)*period
print('duration decoded',len(x)/sr,'estimated BPM',bpm,'offset',offset)
for a in range(0,131,4):
 m=(t>=a)&(t<a+4); print(a, np.round(np.mean(bands[:,m],axis=1),4).tolist())
(p/'public/data').mkdir(parents=True,exist_ok=True)
outt=np.arange(0,len(x)/sr,.01);env=[]
for b in bands:
 b=np.interp(outt,t,b);b=np.clip(b/np.percentile(b,96),0,1);env.append(np.round(b,3).tolist())
data={'sampleStep':.01,'durationDecoded':len(x)/sr,'estimatedBpm':bpm,'estimatedOffset':offset,'bands':env,'onsets':[[round(float(ft[i]),4),round(float(flux[i]/np.percentile(flux,99)),3)] for i in peaks]}
(p/'public/data/envelope.json').write_text(json.dumps(data,separators=(',',':')))
fig,ax=plt.subplots(4,1,figsize=(18,8),sharex=True)
for i in range(3):ax[i].plot(t,bands[i],lw=.5);ax[i].set_ylabel(['35–180 Hz','180–2200 Hz','2.2–10 kHz'][i]);ax[i].grid()
ax[3].plot(ft,flux,lw=.4);ax[3].set_xticks(np.arange(0,132,4));ax[3].grid();ax[3].set_xlabel('seconds');plt.tight_layout();plt.savefig(p/'research/rock-hybrid-analysis.png')
