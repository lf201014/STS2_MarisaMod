shader_type canvas_item;
render_mode blend_sub;

uniform float glow_factor = 4.5;//越大辉光弥散越少。不要让辉光弥散到图外。
//uniform float test = 0.0;

uniform float progress: hint_range(0.0, 1.0) = 0.0f;
//c的x,y,z取值范围均为0~1
vec3 hsv2rgb(vec3 c){
    vec3 rgb = clamp(abs(mod(c.x*6.0 + vec3(0,4,2),6.0)-3.0)-1.0,0.0,1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}
//归一化亮度
vec3 norm_col(vec3 c){
	float lum = dot(c, vec3(0.2126, 0.7152, 0.0722));
	c /= (lum + 1e-4);
	return c;
}

void fragment()
{
	//脉动
	float pulse_period = 1.5;
	float pulse_amplitude = 0.08;
    float pulse = 1.0 + pulse_amplitude * sin(TIME * 6.28318 / pulse_period);

    vec2 uv = (UV - 0.5) * 2.0;
    float r = length(uv);//0~1

	float total_spin = PI*1.25;
	float spin_progress = min(total_spin * progress*2.0,total_spin) ;//期望在后半段不旋转，仅缩放星星大小
    float angle = atan(uv.y, uv.x) +spin_progress;
    float sector = 6.28318 / 4.0;
    float a = mod(angle + sector * 0.5, sector) - sector * 0.5;
    // 旋转到标准方向
    vec2 p = vec2(cos(a), sin(a)) * r;
    // 非各向同性距离
	//float glow_factor = 0.3+ 0.2 * progress;
	float factor3 = 14.0- 7.0 *progress;
	float factor2 = 0.1+ 0.25 * pow(abs(progress-0.5)*2.0,0.4) + 0.15 * progress;
	float factor = -0.8+  1.8* progress;// progress*0.5 +  0.5;
	factor *= pow(pulse,0.3);
	factor2 *= sqrt(pulse);
	factor3 *= pulse;
	//factor= test;
    float d = (pow(abs(p.x),factor2) + factor* pow(abs(p.y),factor2)) ;

	float glow = glow_factor * pulse;
    float rays = pow(1.0 / (d * factor3 + 0.02), glow);

	vec3 hsv = vec3(TIME + angle/(PI * 2.0),0.5,1);
	vec3 color = norm_col(hsv2rgb(hsv));
	color = color * rays;

	float ring_radius = 0.08;
	float ring_sharp = 1.5;//越大就越尖锐越薄
	float ring_brightness = 0.1;

	ring_radius *= 0.6 + 0.4 * 2.0 * abs(progress-0.5);
	ring_radius /= pow(pulse, 2);
	ring_sharp *=0.4 + 0.4* pow(abs(progress-0.4)*2.0,2.0);
	ring_sharp *= pow(pulse,0.5);
	ring_brightness *=  (0.4 + 0.6* progress);
	float ring = exp(-abs((r-ring_radius)*ring_sharp) * 50.0)*ring_brightness;

	color +=  norm_col(hsv2rgb(hsv)) * ring;
    COLOR = vec4(color, 1.0);
}
