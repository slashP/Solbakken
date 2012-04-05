// -----------------------------------------------------------------------------------
// http://wowslider.com/
// JavaScript Wow Slider is a free software that helps you easily generate delicious 
// slideshows with gorgeous transition effects, in a few clicks without writing a single line of code.
// Last updated: 2011-10-27
//
//***********************************************
// Obfuscated by Javascript Obfuscator
// http://javascript-source.com
//***********************************************
function ws_basic(b,a,f){var d=jQuery;var e=f.find("ul");var c=e.children();c.css({position:"relative","float":"left"});f.css({position:"relative",overflow:"hidden"});e.css({position:"relative",width:(b.outWidth*a.length)*1.1+"px",height:"100%",top:0});e.css({left:-d(c.get(b.startSlide)).position().left});a.css({position:"static"});this.go=function(g){e.stop(true).animate({left:-d(c.get(g)).position().left},b.duration,"easeInOutExpo");return g}};// -----------------------------------------------------------------------------------
// http://wowslider.com/
// JavaScript Wow Slider is a free software that helps you easily generate delicious 
// slideshows with gorgeous transition effects, in a few clicks without writing a single line of code.
// Last updated: 2011-10-27
//
//***********************************************
// Obfuscated by Javascript Obfuscator
// http://javascript-source.com
//***********************************************
jQuery("#wowslider-container1").wowSlider({effect:"basic",prev:"",next:"",duration:7*100,delay:15*100,outWidth:640,outHeight:480,width:640,height:480,autoPlay:false,stopOnHover:false,loop:false,bullets:true,caption:true,controls:true,logo:"engine1/loading.gif",images:0});