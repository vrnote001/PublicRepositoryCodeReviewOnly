using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectTool
{
    /// <summary>
    /// 一定期間操作をブロックするパネルを表示するクラス
    /// </summary>
    public class OverRayPanel : IUserInputRestricted
    {
        private static OverRayPanel _instance;
        public static OverRayPanel Instance => _instance ??= new OverRayPanel();

        private readonly Canvas _canvas;
        private readonly CanvasGroup _canvasGroup;
        private int _blockCount = 0;
    
        private  readonly object _lockObject = new object();
    
        private OverRayPanel()
        {
            //RayCastをBlockするための「死なない」Canvasを生成する
            _canvas = new GameObject("OverRayPanel").AddComponent<Canvas>();
            _canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            _canvas.AddComponent<GraphicRaycaster>();

            UnityEngine.Object.DontDestroyOnLoad(_canvas.gameObject);

            _canvasGroup = _canvas.gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;

            var image = _canvas.AddComponent<Image>();
            image.transform.SetParent(_canvas.transform);
            image.color = Color.clear;
        
            _instance = this;
        }

        /// <summary>
        /// 操作のブロックを開始します
        /// 必ず終了後にUnBlockを呼び出してください
        /// </summary>
        public void Block()
        {
            lock (_lockObject)
            {
                //複数呼び出しに対応するため呼び出し数をカウントする
                _canvasGroup.blocksRaycasts = true;
                _blockCount++;
        
                Debug.Log
                    ($"OverRayPanel.Block():BlockCount:[{_blockCount}]!!");
            }
        }

        /// <summary>
        /// 操作のブロックを解除します
        /// </summary>
        public void UnBlock()
        {
            //呼び出し数をデクリメントし、0になった時にBlocksRaycastsを解除する
            //※つまり呼び出し先で必ず呼び出しを行う必要があります
            lock (_lockObject)
            {
                var value = Mathf.Max(_blockCount-1, 0);
                _blockCount = value;
        
                Debug.Log
                    ($"OverRayPanel.Block():UnBlock:[{_blockCount}]!!");

                if (value != 0) return;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 他のブロック状況の呼び出しに関わらずブロックを強制的に解除します
        /// </summary>
        public void ForcedUnBlock()
        {
            lock (_lockObject)
            {
                _canvasGroup.blocksRaycasts = false;
                _blockCount = 0;
                Debug.Log($"OverRayPanel.ForcedUnBlock:UnBlock:[{_blockCount}]!!");
            }
        }
    }
}

