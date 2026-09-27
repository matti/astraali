from PIL import Image,ImageDraw,ImageFont
from pathlib import Path
P=Path('public/data/plates');P.mkdir(exist_ok=True)
F='/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf';M='/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf'
INK='#171b1c';CREAM='#eee9da';ORANGE='#f24c1b'
def new():return Image.new('RGBA',(1920,1080))
def text(im,xy,s,size=28,fill=INK,font=M):ImageDraw.Draw(im).text(xy,s,font=ImageFont.truetype(font,size),fill=fill)
def tall(im,xy,s,w,h,fill=INK):
 f=ImageFont.truetype(F,240);bb=f.getbbox(s);a=Image.new('RGBA',(bb[2]+8,bb[3]-bb[1]+6));ImageDraw.Draw(a).text((0,-bb[1]),s,font=f,fill=fill);a=a.resize((w,h),Image.Resampling.LANCZOS);im.alpha_composite(a,xy)
def save(im,name):im.save(P/(name+'.png'))
def frame(name,num,label,light=False):
 im=new();d=ImageDraw.Draw(im);c=CREAM if light else INK
 text(im,(70,46),'BUREAU OF TEMPORARY MEASURES',22,c);text(im,(1550,46),'TF / '+num+' / ∞',22,c)
 d.line((70,92,1850,92),fill=c,width=2);text(im,(70,994),label,20,c);text(im,(1610,994),'JML ENGINE',20,c)
 save(im,name)
frame('frame0','01','OBJECT 001   /   DURATION: TEMPORARY')
frame('frame1','02','MULTIPLICATION IS NOT PROGRESS',True)
frame('frame2','03','DEPARTMENT OF PENDING DECISIONS',True)
frame('frame3','04','INFRASTRUCTURE WITHOUT A DESTINATION',True)
frame('frame4','05','NO THROUGH ROAD TO THE HEAVENS',True)
frame('frame5','06','TEMPORARY STATUS: PERMANENT',True)
frame('frame6','07','THE WORK ZONE IS NOW THE WORLD',True)
im=new();tall(im,(75,250),'THIS WILL',720,175);tall(im,(75,443),'ONLY TAKE',790,175);tall(im,(75,636),'A MINUTE.',740,175);text(im,(85,869),'PLEASE REMAIN EXACTLY WHERE YOU ARE.',24);save(im,'minute')
im=new();tall(im,(70,230),'TEMPORARY',940,215);tall(im,(70,472),'FOREVER',815,215);text(im,(78,735),'A MONUMENT TO ESSENTIAL NON-PROGRESS',23);text(im,(78,786),'AN ORIGINAL DEMO / 2026',23);save(im,'title')
im=new();tall(im,(80,160),'ONE SMALL',960,165,CREAM);tall(im,(80,342),'OBSTRUCTION.',1150,165,CREAM);save(im,'small')
im=new();tall(im,(85,179),'PLEASE',680,175,CREAM);tall(im,(85,374),'WAIT.',510,175,CREAM);text(im,(90,639),'YOUR FUTURE IS BEING REDIRECTED.',24,CREAM);text(im,(90,687),'ESTIMATED COMPLETION: —',24,CREAM);save(im,'wait')
im=new();tall(im,(80,155),'CIVIL',570,155,CREAM);tall(im,(80,325),'DISOBEDIENCE.',1280,155,CREAM);save(im,'civil')
im=new();tall(im,(80,150),'ROAD CLOSED.',1010,155,CREAM);text(im,(86,337),'INCLUDING ALL ALTERNATIVE REALITIES.',24,CREAM);save(im,'road')
im=new();tall(im,(90,194),'NOTHING IS',790,135,CREAM);tall(im,(90,349),'MORE',450,135,CREAM);tall(im,(90,504),'PERMANENT',835,135,CREAM);save(im,'nothing')
im=new();tall(im,(90,194),'THAN A',540,135,CREAM);tall(im,(90,349),'TEMPORARY',855,135,CREAM);tall(im,(90,504),'SOLUTION.',750,135,CREAM);save(im,'solution')
im=new();tall(im,(100,330),'FOREVER.',1720,300,CREAM);text(im,(108,680),'EXTENSION APPROVED.  NO FURTHER QUESTIONS.',27,CREAM);save(im,'forever')
im=new();tall(im,(80,189),'STILL HERE.',1190,210,CREAM);text(im,(90,473),'TEMPORARY / FOREVER',34,CREAM);text(im,(90,545),'CONCEPT, CODE & PROCEDURAL ART / CODEX',24,CREAM);text(im,(90,590),'MUSIC / ROCK HYBRID — KEVIN MACLEOD',24,CREAM);text(im,(90,635),'INCOMPETECH.COM / CC BY 4.0 / MUSIC UNCHANGED',20,CREAM);text(im,(90,707),'POWERED BY JUMALAUTA / JML-ENGINE-WEBGL',24,CREAM);text(im,(90,785),'GREETINGS TO EVERYONE STILL MAKING DEMOS.',22,CREAM);text(im,(90,835),'THANK YOU FOR YOUR PATIENCE. IT WAS MANDATORY.',22,CREAM);save(im,'credits')
# Blank full-size plane for original shader passes.
Image.new('RGBA',(1920,1080),(255,255,255,255)).save('public/data/plane.png')
