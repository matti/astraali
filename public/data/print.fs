in vec2 texCoord;
out vec4 fragColor;
uniform float time;
uniform float mode;
uniform float pulse;
uniform float beat;
uniform float bass;
uniform float fade;
mat2 rot(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float hash(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
float icon(vec2 p){float body=step(abs(p.x),(.29-p.y*.36))*step(-.43,p.y)*step(p.y,.62);float base=step(abs(p.x),.49)*step(abs(p.y+.49),.065);return max(body,base);}
void main(){
 vec2 uv=(texCoord-.5)*vec2(16./9.,1.);vec3 paper=vec3(.90,.88,.81),ink=vec3(.035,.045,.05),red=vec3(.97,.22,.045);vec3 col=paper;
 if(mode<.5){
 vec2 p=uv-vec2(.48,0.);p*=2.2;float r=length(p);float circle=1.-smoothstep(.006,.013,abs(r-.58));float marks=step(.96,cos(atan(p.y,p.x)*48.))*step(.58,r)*step(r,.64);
 col=mix(paper,ink,circle+marks);p.y+=.07;p*=1.6;float shape=icon(p);col=mix(col,red,shape);float stripe=step(.05,p.y)*step(p.y,.22);col=mix(col,paper,shape*stripe);col=mix(col,ink,shape*step(p.y,-.43));
 float xline=step(abs(uv.y),.0008);col=mix(col,ink,xline*.2);
 }else if(mode<1.5){
 vec2 q=uv; q=rot(sin(beat*.14)*.32)*q;float zoom=5.+floor(mod(beat/4.,3.));q*=zoom;q.y+=beat*.18;
 vec2 id=floor(q);vec2 p=fract(q)-.5;p=rot(mod(id.x+id.y+floor(beat/2.),4.)*1.5707963)*p;p*=1.7;
 float c=icon(p);vec3 bg=mod(floor(beat/4.),2.)<1.?red:paper;col=mix(bg,ink,c);float stripe=step(.04,p.y)*step(p.y,.2);col=mix(col,paper,c*stripe);col=mix(col,red,step(.95,fract(q.x+.12*q.y))*.5);
 }else{
 col=ink;float line=step(abs(uv.y),.001+.002*bass);col+=red*line;float sweep=exp(-abs(uv.x-(fract(time*.35)*2.-1.))*25.);col+=red*sweep*line*3.;
 }
 col+=(hash(gl_FragCoord.xy)-.5)*.035;fragColor=vec4(col*fade,1.);
}
