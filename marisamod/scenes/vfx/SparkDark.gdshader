shader_type canvas_item;
render_mode blend_sub;
//render_mode blend_sub;
//光环
uniform float ring_radius = 0.1;
uniform float ring_factor = 0.4;//越大就越尖锐越薄
uniform float ring_brightness = 0.00;

//星芒
uniform float ray_count = 4.0;
uniform float arm_factor = 1;//越大星芒越粗越远
uniform float arm_factor2 = 0.5;//越小星芒越锐利
uniform float glow_factor = 4;//越大辉光弥散越少。不要让辉光弥散到图外。

//脉动
uniform float pulse_period = 3.5;
uniform float pulse_amplitude = 0.25;

//颜色

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
	//脉动系数
    float pulse = 1.0 + pulse_amplitude * sin(TIME * 6.28318 / pulse_period);

    vec2 uv = (UV - 0.5) * 2.0;
    float r = length(uv);//0~1
    float angle = atan(uv.y, uv.x);
    float sector = 6.28318 / ray_count;
    float a = mod(angle + sector * 0.5, sector) - sector * 0.5;
    // 旋转到标准方向
    vec2 p = vec2(cos(a), sin(a)) * r;
    // 非各向同性距离
    float d = (pow(abs(p.x),arm_factor2) + pulse * arm_factor* pow(abs(p.y),arm_factor2))/ (arm_factor* (1.0+pulse_amplitude)) ;
    float rays = pow(1.0 / (d * 8.0 + 0.02), glow_factor);
    // 光环
    float ring = exp(-abs((r-ring_radius*pulse)*ring_factor*pulse) * 50.0)*ring_brightness;

    //vec3 color = star_color.rgb * rays + vec3(1.0) * ring;
	vec3 hsv = vec3(TIME/ pulse_period/2.0 + angle/(PI * 2.0),0.7,1);//此处的亮度设置没有意义，后面会对亮度归一化
	vec3 color = norm_col(hsv2rgb(hsv));
	color = color * rays + vec3(1.0) * ring;
    COLOR = vec4(color, 1.0);
}