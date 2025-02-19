 Mandle_10Minute_Game\Neglect\Library\ScriptAssemblies
위 경로에 
- Unity.InputSystem.dll
- Unity.InputSystem.pdb

이 2개의 파일을 덮어씌우기 한다.
InputSystemUIInputModule의 코드를 수정한 것으로
private RaycastResult PerformRaycast(ExtendedPointerEventData eventData) 함수 내부의 excludeLayer를 추가해 상호작용에서 특정 레이어를 제외하도록 도와준다.