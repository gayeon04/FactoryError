
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewProductData",
    menuName = "Factory/Product Data")]
public class ProductData : ScriptableObject
{
    public ProductType productType;

    [Header("Visual")]
    public Color color = Color.white;

    [Header("Physics")]
    [Min(0.001f)]
    public float mass = 1f;

    public float radius = 0.25f;

    [Range(0f, 1f)]
    public float restitution = 0.5f;

    [Header("Material")]
    public bool isMagnetic;
    public bool isFragile;
    public float breakThreshold = 5f;

    [Header("Score")]
    public int score = 100;
}
