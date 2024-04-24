using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IDetectable : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    void OnVisible(bool on);
}
