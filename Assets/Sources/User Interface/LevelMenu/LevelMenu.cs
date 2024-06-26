using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem.UI;

public class LevelMenu : MonoBehaviour
{
    [SerializeField] RectTransform _container;
    [SerializeField] Button _buttonTemplate;
    [SerializeField] bool _unlockAllLevels;
    [SerializeField] InputSystemUIInputModule _uiModule;
    [HideInInspector][SerializeField] string[] _levelIds;

    private void CreateButtons()
    {
        var pass = true;
        LevelRepository.Get(_levelIds, Create);
        void Create(List<LevelData> dataList) { for (var i = 0; i < dataList.Count; i++) { CreateButton(i, dataList[i]); } }
        void CreateButton(int index, LevelData levelData)
        {
            var button = Instantiate(_buttonTemplate, _container);
            button.GetComponentInChildren<TMP_Text>().text = $"{index + 1}";
            button.GetComponentInChildren<StarCounterSetter>().DisplayStarsCount(levelData.Stars);
            if (!pass && !_unlockAllLevels) { button.GetComponentInChildren<StarCounterSetter>().DisplayLock(); }
            else { button.onClick.AddListener(() => {_uiModule.enabled = false; LevelManager.Load(CreateLevelContext(index));});}
            if (!levelData.Passed) { pass = false; }
        }
    }

    private LevelContext CreateLevelContext(int index) => new(index, Array.AsReadOnly(_levelIds), SceneManager.GetActiveScene().name);

    private void Awake() => CreateButtons();

#if UNITY_EDITOR
    [SerializeField] SceneAsset[] _levelAssets;
    private void OnValidate()
    {
        _levelIds = _levelAssets
            .Select(scene => scene.name)
            .ToArray();
    }
#endif
}
