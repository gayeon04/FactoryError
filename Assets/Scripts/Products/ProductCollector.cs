
using System.Diagnostics;
using UnityEngine;

public class ProductCollector : MonoBehaviour
{
    [Header("Collect Settings")]
    public Vector2 collectSize = Vector2.one;

    public bool acceptAll = true;
    public ProductType targetType;

    [Header("Runtime")]
    public int collectedCount;
    public int failedCount;
    public int score;

    public void CheckProduct(Product product)
    {
        if (product == null ||
            product.IsCollected ||
            product.Data == null)
            return;

        Vector2 diff =
            (Vector2)(product.transform.position
                     - transform.position);

        Vector2 halfSize = collectSize * 0.5f;

        // 수거 영역 내부인지 검사
        if (Mathf.Abs(diff.x) > halfSize.x ||
            Mathf.Abs(diff.y) > halfSize.y)
            return;

        bool isValid =
            (acceptAll ||
             product.Data.productType == targetType)
            && !product.IsBroken;

        int productScore = product.Data.score;

        if (!product.TryCollect())
            return;

        if (isValid)
        {
            collectedCount++;
            score += productScore;
        }
        else
        {
            failedCount++;
            score -= 50;
        }

        UnityEngine.Debug.Log(
            $"Collected: {collectedCount}, " +
            $"Failed: {failedCount}, " +
            $"Score: {score}"
        );
    }

    public void ResetCollector()
    {
        collectedCount = 0;
        failedCount = 0;
        score = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            (Vector3)collectSize
        );
    }
}
