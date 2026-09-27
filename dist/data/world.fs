in vec2 texCoord;
out vec4 fragColor;
uniform float time;
uniform float act;
uniform float local;
uniform float bass;
uniform float treble;
uniform float pulse;
uniform float beat;
uniform float fade;
#define PI 3.14159265359
mat2 rot(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float hash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
float box(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}
vec2 nearD(vec2 a,vec2 b){return a.x<b.x?a:b;}
vec2 cone(vec3 p){
 float r=.77-.29*p.y;
 float body=max((length(p.xz)-r)*.95,max(.17-p.y,p.y-2.32));
 float stripe=step(.87,p.y)*step(p.y,1.18)+step(1.67,p.y)*step(p.y,1.94);
 vec2 d=vec2(body,mix(1.,2.,stripe));
 d=nearD(d,vec2(box(p-vec3(0,.09,0),vec3(.88,.09,.88))-.035,3.));
 // Raised collar surrounding the little hollow top.
 float rim=length(vec2(length(p.xz)-.105,p.y-2.31))-.034;
 return nearD(d,vec2(rim,1.));
}
vec2 map(vec3 p){
 vec3 q=p;vec2 d=vec2(100.,0.);
 if(act<.5){q.xz=rot(.2*time)*q.xz;d=cone(q);d=nearD(d,vec2(box(p-vec3(0,-.38,0),vec3(1.24,.28,1.24))-.04,4.));}
 else if(act<1.5){q.z+=time*2.1;vec2 id=floor((q.xz+1.65)/3.3);q.xz=mod(q.xz+1.65,3.3)-1.65;q.y-=.2*pow(max(0.,sin(beat*PI-id.x*.5-id.y*.6)),3.);q.xz=rot(.12*sin(id.x+id.y+time))*q.xz;d=cone(q);}
 else if(act<2.5){q.xz=rot(time*.14)*q.xz;d=cone(q);}
 else if(act<3.5){q.z+=local*3.0;vec2 id=floor((q.xz+3.)/6.);q.xz=mod(q.xz+3.,6.)-3.;float s=1.6+hash(id)*2.7;q.y/=s;d=cone(q);d.x*=.72;}
 else if(act<4.5){q.xz=rot(time*.26)*q.xz;float a=atan(q.z,q.x);float sector=2.*PI/12.;float ca=floor((a+sector*.5)/sector)*sector;q.xz=rot(ca)*q.xz;q.x-=4.4;q.xy=rot(.6+sin(local*.22)*.3)*q.xy;q.y+=.9;d=cone(q);float hoop=length(vec2(length(p.xz)-4.5,p.y+.5))-.095;d=nearD(d,vec2(hoop,2.));}
 else if(act<5.5){
 q.xz=rot(time*.24)*q.xz;q.xy=rot(.16*sin(time*.37))*q.xy;
 for(int i=0;i<9;i++){float fi=float(i);float height=.28+fi*.245;float spread=smoothstep(0.,9.,local)*1.5;vec3 r=q-vec3(.18*sin(time*.5+fi)*spread,(height-1.1)*(1.+spread)+1.,0.);r.xz=rot(fi*.2+time*.3)*r.xz;float orbit=smoothstep(10.,22.,local);r.yz=rot(orbit*(fi*.43+time*.17))*r.yz;float radius=(.77-.29*height)*(1.+orbit*.6);float ring=length(vec2(length(r.xz)-radius,r.y))-.095;d=nearD(d,vec2(ring,mod(fi,3.)<1.?2.:1.));}
 d=nearD(d,vec2(box(q-vec3(0,-1.8*smoothstep(0.,9.,local),0),vec3(.9,.08,.9)),3.));
 }else{q.xz=rot(time*.13)*q.xz;float s=2.3;d=cone(q/s);d.x*=s;vec3 r=p;r.y-=2.6;float a=atan(r.z,r.x);float ca=floor((a+PI/4.)/(PI/2.))*(PI/2.);r.xz=rot(ca)*r.xz;r.x-=4.8;r.xy=rot(-PI*.5)*r.xy;r.y+=1.1;d=nearD(d,cone(r));}
 if(act<3.5 || act>5.5){float ground=act<.5?-.72:act<2.5?-.18:-.16;d=nearD(d,vec2(p.y-ground,0.));}
 return d;
}
vec3 normal(vec3 p){vec2 e=vec2(.003,0);return normalize(vec3(map(p+e.xyy).x-map(p-e.xyy).x,map(p+e.yxy).x-map(p-e.yxy).x,map(p+e.yyx).x-map(p-e.yyx).x));}
float shadow(vec3 p,vec3 l){float sh=1.,t=.03;for(int i=0;i<18;i++){float h=map(p+l*t).x;sh=min(sh,12.*h/t);t+=clamp(h,.05,.5);if(t>7.)break;}return clamp(sh,.15,1.);}
void main(){
 vec2 uv=(texCoord-.5)*vec2(16./9.,1.);vec2 screen=uv;vec3 ro,ta;float lens=1.7;float roll=0.;
 vec3 paper=vec3(.86,.835,.76),ink=vec3(.018,.025,.031),orange=vec3(1.,.155,.025),blue=vec3(.025,.09,.42);
 if(act<.5){uv.x-=.34;float a=.55+time*.12;ro=vec3(5.5*sin(a),3.1,5.5*cos(a));ta=vec3(0,.8,0);lens=1.23;}
 else if(act<1.5){float overhead=smoothstep(6.,9.,local)*(1.-smoothstep(12.,16.,local));ro=mix(vec3(7.+sin(local*.16)*2.,6.5,9.),vec3(.5,14.,3.),overhead);ta=vec3(0,.1,-3.);lens=1.5;roll=.08*sin(local*.13);}
 else if(act<2.5){ro=vec3(3.4,2.4,4.5);ta=vec3(0,1.,0);lens=1.2;uv.x-=.43;}
 else if(act<3.5){ro=vec3(3.,1.25+.3*sin(local*.4),6.);ta=vec3(2.3,3.4,-8.);lens=1.05;roll=.07*sin(local*.25);}
 else if(act<4.5){float a=local*.19;ro=vec3(8.*sin(a),6.+3.*sin(local*.2),8.*cos(a));ta=vec3(0,.3,0);lens=1.02;roll=local*.065;}
 else if(act<5.5){float a=local*.15;ro=vec3(5.5*sin(a),2.2,5.5*cos(a));ta=vec3(0,.8,0);lens=.95;uv.x-=.38;}
 else {float cut=mod(floor(beat/8.),3.);float a=local*.12+cut*.6;ro=vec3(8.3*sin(a),2.3+cut*1.2,8.3*cos(a));ta=vec3(0,2.5,0);lens=1.25;roll=.06*sin(beat*PI/4.);}
 uv=rot(roll)*uv;uv*=1.-pulse*.017;
 vec3 fw=normalize(ta-ro),rt=normalize(cross(fw,vec3(0,1,0))),up=cross(rt,fw);vec3 rd=normalize(fw*lens+uv.x*rt+uv.y*up);
 vec3 bg=paper;
 if(act>.5&&act<1.5)bg=blue*(.7+max(rd.y,0.));
 if(act>2.5&&act<3.5)bg=mix(vec3(.8,.16,.04),paper,pow(max(0.,rd.y),.4));
 if(act>3.5)bg=ink;
 if(act>1.5&&act<2.5)bg=blue;
 float distance=0.;vec2 hit;vec3 p;
 for(int i=0;i<80;i++){p=ro+rd*distance;hit=map(p);if(hit.x<.0025*max(1.,distance*.12)||distance>65.)break;distance+=hit.x*.82;}
 vec3 col=bg;
 if(distance<65.){
 vec3 n=normal(p),l=normalize(vec3(-.7,1.4,1.));float diff=max(dot(n,l),0.);float ao=clamp(map(p+n*.23).x/.23,.2,1.);float sh=shadow(p+n*.02,l);vec3 albedo;
 if(hit.y<.5){
 albedo=act<.5?paper:ink;
 float grid=max(1.-smoothstep(.018,.04,abs(fract(p.x*.5+.5)-.5)),1.-smoothstep(.018,.04,abs(fract(p.z*.5+.5)-.5)));
 albedo=mix(albedo,act<.5?vec3(.63,.61,.54):vec3(.17,.18,.16),grid*.55);
 if(act>2.5&&act<3.5){float line=step(abs(p.x-3.),.045)*step(.45,fract((p.z+local*3.)*.3));albedo=mix(albedo,paper,line);}
 if(act>5.5){float ring=1.-smoothstep(.03,.06,abs(mod(length(p.xz)-time*.3,1.8)-.9));albedo+=orange*ring*.6;}
 }else if(hit.y<1.5)albedo=orange;
 else if(hit.y<2.5)albedo=vec3(.91,.91,.82);
 else if(hit.y<3.5)albedo=ink;
 else albedo=vec3(.61,.60,.55);
 float spec=pow(max(dot(n,normalize(l-rd)),0.),45.);float rim=pow(1.-max(dot(n,-rd),0.),3.);
 col=albedo*(.36+diff*sh*.85)*ao+spec*sh*vec3(1.,.88,.72)*.9+rim*vec3(.23,.31,.47)*.43;
 if(act>3.5){col+=rim*orange*.4;col+=albedo*bass*.13;}
 col=mix(col,bg,1.-exp(-distance*distance*.00065));
 if(act>1.5&&act<2.5){float lines=1.-smoothstep(.02,.06,abs(fract(p.y*9.)-.5));col=mix(blue*.7,vec3(.63,.79,.87),clamp((lines*.45+rim)*1.2,0.,1.));}
 }else if(act>3.5){
 vec2 st=floor(uv*vec2(380.,380.));float star=step(.9975,hash(st));col+=star*(.3+.2*sin(time+hash(st)*34.));
 }
 if(act>3.5&&act<4.5){float halo=pow(max(0.,1.-abs(length(uv)-.34)*9.),4.);col+=orange*halo*.055*(1.+bass);}
 // Ink grain and a very light lens falloff, kept consistent across the piece.
 col+=(hash(gl_FragCoord.xy+fract(time)*100.)-.5)*.022;
 col*=1.-.2*pow(length(screen*.8),2.);
 col=pow(max(col,0.),vec3(.92));
 fragColor=vec4(col*fade,1.);
}
