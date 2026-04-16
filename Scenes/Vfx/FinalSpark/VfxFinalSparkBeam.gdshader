shader_type canvas_item;
render_mode blend_add;

uniform sampler2D noise_tex;

//辉光
uniform float glow_factor =1.5;
uniform float glow_factor2 = 1.7;
uniform float rainbow_ratio = 1.0;//彩虹的情况将其设为1.0
uniform vec4 beam_color : source_color = vec4(0.13, 0.4, 0.7, 1.0);

uniform float progress: hint_range(0.0, 1.0) = 1.0;//取值范围0~2，0的时候最小，1的时候最大。


void vertex() {
    float new_x = VERTEX.x;
	float x = 0.0001;
	float stretch_factor = 0.3;
	float t = pow(UV.x+x,stretch_factor)-pow(x,stretch_factor);
	//float t = sqrt(UV.x);

	float grow_progress = smoothstep(0.0,1.0,progress)*0.9;
	//shader内动画预览
	//float grow_time = 1.0;
	//progress = mod(TIME,grow_time) / grow_time;
	float grow_factor = 1.0/(grow_progress+0.001)-0.85;
	float width =grow_factor ;
    float new_y = VERTEX.y/ abs(VERTEX.y) * pow(abs(VERTEX.y),width)*t/2.0;
    VERTEX = vec2(new_x, new_y);
}

vec3 rainbow_uniform(float t) {
    // t 范围 [0,1]，输出更均匀的彩虹
    vec3 rgb;
    rgb.r = sin(t * 6.28318 + 0.0);
    rgb.g = sin(t * 6.28318 + 2.09439);
    rgb.b = sin(t * 6.28318 + 4.18879);

    // 映射负值到 [0,1]
    rgb = (rgb + 1.0) * 0.5;

    // 可选：调整饱和度和亮度
    float saturation = 0.8;
    float value = 1.0;
    return value * (rgb * saturation + vec3(1.0 - saturation));
}
vec3 rainbow_color(vec2 pos)
{
	float noise_speed = 0.9;
	float noise_ratio = 0.8;
	float noise =noise_ratio * texture(noise_tex, vec2(pos.x+TIME/20.0+0.2,pos.y + TIME * noise_speed+0.3)).r;

	float period = 0.8;
	float phase = -pos.x*0.5+ pos.y + mod(TIME,period) / period;
	//phase += noise ;
	vec3 rainbow = rainbow_uniform(phase);
	return rainbow;
}
float glow(float d) {
	float pulse = 1.0+ 0.05* sin(TIME*10.0);
	float brightness = pow(1.0 / (d * glow_factor *pulse + 0.002), glow_factor2) ;
	brightness -= pow( glow_factor*pulse+ 0.002, -glow_factor2);//d= 1时亮度应该收敛到0
	return max(brightness,0.0);
}
void fragment() {
	//获取噪声
	float noise_period = 10.0;
	vec2 noise_uv = vec2( mod(TIME,noise_period)/noise_period , UV.y );
	float n1 = texture(noise_tex, noise_uv).r;
	float n2 = texture(noise_tex, noise_uv * 2.0 + 0.3).r;
	float noise = mix(n1, n2, 0.5);//理论上说这里noise取值应该在0~1之间

	//脉动
	float pulse_period = 2.5;
	float pulse_amplitude = 0.15;
    float pulse = 1.0 + pulse_amplitude * sin(TIME * 6.28318 / pulse_period);
    float d =(abs(UV.y -0.5)*2.0 +noise*0.2)/(1.0+noise*0.2);
	float glow = glow(d);
	vec3 color = beam_color.rgb;
	color = mix(color,rainbow_color(UV),rainbow_ratio) ;
	color = color * glow;
    COLOR = vec4(color, 1.0);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
