using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (TextMesh))]
[ExecuteInEditMode]
public class OutlineTextMesh : MonoBehaviour
{
	public enum ShaderType {
		Normal,
		Outline,
	}

	public ShaderType shaderType = ShaderType.Outline;
	public Color outlineColor = Color.black;
	public float outerThickness = 1.0f;
	public float innerThickness = 1.0f;

	const string ShaderName_Normal = "GUI/Text Shader";
	const string ShaderName_Outline = "GUI/Outline Text Shader";
	string _shaderName {
		get {
			switch( this.shaderType ) {
			case ShaderType.Normal:		return ShaderName_Normal;
			case ShaderType.Outline:	return ShaderName_Outline;
			}
			return "";
		}
	}

	MeshRenderer _meshRenderer;
	Material _material;
	TextMesh _textMesh;
	Color _cached_outlineColor = new Color( -1.0f, -1.0f, -1.0f, -1.0f );
	float _cached_outerThickness = -1.0f;
	Vector4 _cached_innerThickness = new Vector4( -1.0f, -1.0f, -1.0f, -1.0f );

	public void ForceUpdate()
	{
		if( _meshRenderer == null ) {
			_material = null;
			_meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
		}
		if( _textMesh == null ) {
			_textMesh = this.gameObject.GetComponent<TextMesh>();
		}
		
		if( _meshRenderer != null ) {
			_material = _meshRenderer.sharedMaterial;
		}

		string shaderName = _shaderName;
		if( _material == null || _material.shader == null || _material.shader.name != shaderName ) {
			if( _textMesh != null && _textMesh.font != null ) {
				if( this.shaderType == ShaderType.Normal ) {
					_material = _textMesh.font.material;
				} else {
					_material = _textMesh.font.material;
					_material.shader = Shader.Find( shaderName );
				}
				if( _meshRenderer != null ) {
					_meshRenderer.sharedMaterial = _material;
				}
			}
		}

		if( _material != null && _material.shader != null && _material.shader.name == ShaderName_Outline ) {
			Texture mainTexture = _material.mainTexture;
			if( mainTexture != null && _textMesh != null ) {
				Texture2D mainTexture2D = mainTexture as Texture2D;
				if( mainTexture2D != null ) {
					float fontSize = (float)_textMesh.fontSize;
					float width = Mathf.Max( (float)mainTexture2D.width, 1.0f );
					float height = Mathf.Max( (float)mainTexture2D.height, 1.0f );

					float shader_outerThickness = this.outerThickness * 0.005f;

					Vector4 shader_innerThickness = new Vector4(
						this.innerThickness * 0.005f * fontSize / width,
						this.innerThickness * 0.005f * fontSize / height,
						0.0f, 0.0f );

					if( _cached_outlineColor != this.outlineColor ) {
						_cached_outlineColor = this.outlineColor;
						if( _material.GetColor( "_OutlineColor" ) != this.outlineColor ) {
							_material.SetColor( "_OutlineColor", this.outlineColor );
						}
					}
					if( _cached_outerThickness != shader_outerThickness ) {
						_cached_outerThickness = shader_outerThickness;
						if( _material.GetFloat( "_OuterThickness" ) != shader_outerThickness ) {
							_material.SetFloat( "_OuterThickness", shader_outerThickness );
						}
					}
					if( _cached_innerThickness != shader_innerThickness ) {
						_cached_innerThickness = shader_innerThickness;
						if( _material.GetVector( "_InnerThickness" ) != shader_innerThickness ) {
							_material.SetVector( "_InnerThickness", shader_innerThickness );
						}
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		ForceUpdate();
	}
}
