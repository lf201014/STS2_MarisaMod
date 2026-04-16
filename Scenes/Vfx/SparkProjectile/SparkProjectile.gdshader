shader_type canvas_item;
render_mode blend_add;
uniform sampler2D noise_tex;
uniform sampler2D noise_tex2;
uniform float flow_speed = 0.4;
uniform float noise_scale = 5.0;

uniform vec4 spark_color : source_color = vec4(0.13, 0.4, 0.7, 1.0);
uniform float rainbow_ratio = 0.05;
vec3 hsv2rgb(vec3 c){
    vec3 rgb = clamp(abs(mod(c.x*6.0 + vec3(0,4,2),6.0)-3.0)-1.0,0.0,1.0);
    return c.z * mix(vec3(1.0), rgb, c.y);
}
vec3 hsv2rgb_smooth(vec3 c)
{
    vec3 rgb = clamp(abs(mod(c.x+vec3(0.,4.,2.),6.)-3.)-1.,0.,1.);
    rgb = rgb*rgb*(3.0-2.0*rgb); // iq's cubic smoothing.
    return c.z+ c.y*(rgb-0.5)*(1.-abs(2.*c.z-1.));
}

void fragment()
{float u = UV.x;
float v = UV.y;

// ===== 横向结构（正确对称）=====
float center_dist = abs(v - 0.5) * 2.0;

float core = smoothstep(0.0, 0.3, 1.0 - center_dist);
float rim = smoothstep(0.4, 1.0, center_dist);

// ===== 噪声 =====
vec2 noise_uv = vec2(u * noise_scale - TIME * flow_speed, v);
float n1 = texture(noise_tex, noise_uv).r;
float n2 = texture(noise_tex, noise_uv * 2.0 + 0.3).r;
float noise = mix(n1, n2, 0.5);
noise = smoothstep(0.3, 0.8, noise);

float hue = fract(u + TIME * 0.2);
float sat = mix(0.1, 0.4, center_dist);
float val = mix(1.1, 0.9, center_dist);

vec3 color = hsv2rgb(vec3(hue, sat, val));


color *= mix(0.85, 1.15, noise);

float fade = pow(u, 1.5);
float alpha = fade * (core + rim * 0.6);
alpha *= mix(0.7, 1.3, noise);

COLOR = vec4(color, alpha);
}