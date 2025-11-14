using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChainLightningVFX : MonoBehaviour
{
    private Vector3 mainTargetPos;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private VisualEffect sparkVFXPrefab;
    [SerializeField] private float chainRange = 5f;
    [SerializeField] private float delayPerChain = 0.1f;
    [SerializeField] private float chainDuration = 0.5f;

    private bool isActive;
    private float elapsedTime = 0f;
    private const string VFX_EVENT_NAME = "OnAbilityCasted";
    public List<EnemyBase> enemiesInChain;
    
    [Header("DEBUG")]
    public bool destroyChains = true;
    public bool clickToTest = false;

    /*private void Update() {
        if (!clickToTest) return;

        if(Input.GetMouseButtonDown(0)) {
            EnemyBase closestEnemyToClick = EnemyManager.GetClosestEnemy(GameManager.GetMousePos(), 2f);
            if (closestEnemyToClick) ActivateChain(closestEnemyToClick.transform.position, chainRange);
        }
    }*/

    public void ActivateChain(List<EnemyBase> enemies, float chainRange = -1) {
        if (chainRange > -1) this.chainRange = chainRange;
        //mainTargetPos = mainPos;
        isActive = true;
        elapsedTime = 0f;
        enemiesInChain.Clear();
        DelayedChainEffect(enemies);
    }
    private void DelayedChainEffect(List<EnemyBase> enemies) {
        if (isActive) {
            StartCoroutine(ChainedLightning(enemies));
        }
    }
    private IEnumerator ChainedLightning(List<EnemyBase> enemies) {

        if (isActive && elapsedTime < chainDuration) {
            elapsedTime += Time.deltaTime + delayPerChain;

            for (int i = 0; i < enemies.Count-1; i++) {
                if (!enemies[i] || !enemies[i + 1]) continue;
                SpawnRenderer(enemies[i].transform.position, enemies[i+1].transform.position);
                enemiesInChain.Add(enemies[i]);
                yield return new WaitForSeconds(delayPerChain);
            }
        }
    }

        /*private IEnumerator ChainedLightning(Vector3 startTarget) {

            if (isActive && elapsedTime < chainDuration) {
                elapsedTime += Time.deltaTime + delayPerChain;

                EnemyBase closestEnemy = EnemyManager.GetClosestEnemyExcludingSelf(EnemyManager.GetClosestEnemy(startTarget, chainRange), startTarget, chainRange);

                if (closestEnemy != null) {
                    SpawnRenderer(startTarget, closestEnemy.transform.position);
                    enemiesInChain.Add(closestEnemy);
                    yield return new WaitForSeconds(delayPerChain);

                    if (elapsedTime >= chainDuration) yield break;
                    if (!closestEnemy) yield break;

                    EnemyBase nextClosestEnemy = EnemyManager.GetClosestEnemyExcludingSelf(EnemyManager.GetClosestEnemy(closestEnemy.transform.position, chainRange), closestEnemy.transform.position, chainRange);

                    if (!nextClosestEnemy) yield break;

                    if (!enemiesInChain.Contains(nextClosestEnemy)) {
                        StartCoroutine(ChainedLightning(closestEnemy.transform.position));
                    }
                }
                else {
                    SpawnRenderer(startTarget, startTarget); // if self
                    isActive = false;
                }
            }        
        }*/
        private void SpawnRenderer(Vector3 startPos, Vector3 endPos) {
        GameObject lr = Instantiate(lineRendererPrefab);
        if (destroyChains) lr.GetComponent<DelayedDeath>().DeathDelay = chainDuration;
        else lr.GetComponent<DelayedDeath>().CanDie = false;

        lr.GetComponent<LineRendererController>().SetPosition(startPos, endPos);
        VisualEffect vfx = Instantiate(sparkVFXPrefab);
        VisualEffect vfx2 = Instantiate(sparkVFXPrefab);
        vfx.transform.position = startPos;
        vfx2.transform.position = endPos;
        vfx.SendEvent(VFX_EVENT_NAME);
        vfx2.SendEvent(VFX_EVENT_NAME);
    }
}
