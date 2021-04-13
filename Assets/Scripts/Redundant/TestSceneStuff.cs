using UnityEngine;

public class TestSceneStuff : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Play("test_music_don't_sue", 1, 1, true);
    }
}
