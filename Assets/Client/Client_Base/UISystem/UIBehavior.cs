// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public struct UIBehaviorRef<T> where T : UIBehavior
// {
//     T behavior;
//     public T Value
//     {
//         get
//         {
//             if (behavior == null)
//                 behavior = UIBehavior.GetUIBehaviour<T>();
//             return behavior;
//         }
//     }
// }
//
// public class SingletonUIBehaviour<T> : UIBehavior where T : UIBehavior
// {
//     private static T _instance;
//
//     public static T instance { get { return _instance; } }
//
//     protected void Awake()
//     {
//         _instance = this as T;
//     }
// }
//
// public class UIMonoBehaviour : MonoBehaviour
// {
//
// }
// //特殊处理返回时需要继承自这个
// public interface IBackClickHandler
// {
//     void OnBackHandle();
// }
// //特殊处理返回主页时需要继承自这个
// public interface IHomeClickHandler
// {
//     void OnHomeHandle();
// }
// //UI界面的显示或者隐藏时，有特殊处理要继承自这个
// public interface IVisibleHandler
// {
//     void OnVisible(bool isOpen);//是否是打开的时候
//     void OnDisVisible(bool isClose);//是否是关闭的时候
// }
// public class UIBehavior : MonoBehaviour
// {
//     private static Transform mRoot;
//     private static RectTransform mRootRect;
//     private static RectTransform mWindowRoot;
//     private static Camera _camera;
//     public static Camera UICamera
//     {
//         get
//         {
//             if (_camera == null)
//             {
//                 _camera = Root.parent.GetComponent<Camera>();
//             }
//             return _camera;
//         }
//     }
//
//     public static Transform Root
//     {
//         get
//         {
//             if (mRoot == null)
//             {
//                 GameObject goRoot = GameObject.Find("UIRoot");
//                 mRoot = (goRoot == null ? new GameObject("UIRoot") : goRoot).transform;
//                 DontDestroyOnLoad(mRoot.gameObject);
//                 mRootRect = mRoot.GetComponent<RectTransform>();
//             }
//             return mRoot;
//         }
//     }
//
//     public static RectTransform WindowRoot
//     {
//         get
//         {
//             if (mWindowRoot == null)
//             {
//                 var tr = Root.Find("Windows");
//                 if (tr != null)
//                 {
//                     mWindowRoot = tr.GetComponent<RectTransform>();
//                 }
//                 else
//                 {
//                     var windowGo = new GameObject("Windows", typeof(RectTransform));
//                     windowGo.transform.SetParent(Root.transform);
//                     mWindowRoot = windowGo.GetComponent<RectTransform>();
//                     mWindowRoot.localPosition = Vector3.zero;
//                     mWindowRoot.localScale = Vector3.one;
//                     mWindowRoot.anchorMin = Vector2.zero;
//                     mWindowRoot.anchorMax = Vector2.one;
//                     mWindowRoot.sizeDelta = Vector2.zero;
//                     mWindowRoot.anchoredPosition3D = Vector3.zero;
//
//                 }
//             }
//             return mWindowRoot;
//         }
//     }
//
//     public static RectTransform RootRect
//     {
//         get
//         {
//             return mRootRect;
//         }
//     }
//
//     private bool isVisable = false;
//
//     public Table_UIWnd wndInfo;
//
//     public delegate Task OnPreShowUI(UIBehavior wnd);
//     public static OnPreShowUI PreShowUiHandler;
//     public delegate void OnShowUI(UIBehavior wnd, object[] args);
//     public static OnShowUI PostShowUiHandler;
//     public delegate void OnHideUI(UIBehavior wnd);
//     public static OnHideUI HideUiHandler;
//     public delegate void OnLoadUI(UIBehavior wnd);
//     public static OnLoadUI LoadUiHandler;
//     public delegate void OnShowMainWnd(UIBehavior wnd);
//     public static OnShowMainWnd ShowMainWndHandler;
//
//     public delegate void OnLoadScene(UIBehavior wnd, Action action);
//     public static OnLoadScene LoadSceneHandler;
//
//     public static Func<GameObject, bool, Task> SetVisiableMethod;
//
//     public static Action<GameObject> OnLoadPrefb;
//
//     public static Action<Table_UIWnd> OnBeforeLoadPrefab;
//
//     public static Action<UIBehavior> defaultHelpMethod;
//
//     private List<UIBehavior> _childrens = new List<UIBehavior>();
//
//     private Dictionary<string, UnityEngine.Object> _elements = new Dictionary<string, UnityEngine.Object>();
//
//     public void DispatchEvent(ModuleEvent __mEvent)
//     {
//         EventManager.DispatchEvent(__mEvent);
//     }
//
//     public int sortingOrder { get; set; }
//
//     public bool IsVisible
//     {
//         get
//         {
//             return isVisable;
//         }
//     }
//
//     public Task SetVisiable(bool value)
//     {
//         isVisable = value;
//         if (SetVisiableMethod != null && this.gameObject != null)
//         {
//             return SetVisiableMethod(this.gameObject, value);
//         }
//         return Task.CompletedTask;
//     }
//
//     public Task SetVisiable2(bool value)
//     {
//         if (SetVisiableMethod != null && this.gameObject != null)
//         {
//             return SetVisiableMethod(this.gameObject, value);
//         }
//         return Task.CompletedTask;
//     }
//
//     public bool IsShowing
//     {
//         get
//         {
//             return IsShow(wndInfo.Name);
//         }
//     }
//
//     public Dictionary<string, UnityEngine.Object> GetElementContainer()
//     {
//         return _elements;
//     }
//
//     public T GetUIComponent<T>(string name) where T : UnityEngine.Object
//     {
//         if (_elements.ContainsKey(name))
//         {
//             return _elements[name] as T;
//         }
//         else
//         {
//             Debug.LogError("UIElementContainer未找到该组件！");
//             return null;
//         }
//     }
//
//     public void SetElementContainer(string name, UnityEngine.Object component)
//     {
//         if (_elements.ContainsKey(name))
//         {
//             _elements[name] = component;
//         }
//         else
//         {
//             _elements.Add(name, component);
//         }
//     }
//
//     #region LKAdd
//     public void SetActive(GameObject obj, bool bol)
//     {
//         if (obj != null && obj.activeSelf != bol)
//             obj.SetActive(bol);
//     }
//     public T Fd<T>(string path, Transform tr = null) where T : Component
//     {
//         if (tr == null)
//             tr = transform;
//         var tran = tr.Find(path);
//         if (tran == null)
//             return null;
//         return tran.GetComponent<T>();
//     }
//     #endregion
//
//     #region UI调用管理
//     protected virtual void OnShowImp(params object[] args) { }
//     protected virtual Task OnShowImpAsync(params object[] args) { return Task.CompletedTask; }
//     protected virtual Task OnPreShowAsync(params object[] args) { return Task.CompletedTask; }
//
//     public virtual Animator GetAnim() { return this.transform.GetComponent<Animator>(); }
//
//     protected virtual void OnHideImp() { }
//
//     public virtual void OnHelpImp()
//     {
//         defaultHelpMethod?.Invoke(this);
//     }
//
//     public void Show()
//     {
//         TaskUtil.Run(ShowImp());
//     }
//     public Task ShowAsync()
//     {
//         return ShowImp();
//     }
//
//     //给Button的onClick函数调用
//     public void ShowUI_ByName(string uiName)
//     {
//         ShowUIByName(uiName);
//     }
//     //给Button的onClick函数调用
//     public void HideUI_ByName(string uiName)
//     {
//         HideUI(uiName);
//     }
//
//     protected async Task ShowImp(params object[] args)
//     {
//         try
//         {
//             transform.localPosition = new Vector3(-50000, -50000, 0);
//             if (IsShowing)
//             {
//                 try
//                 {
//                     OnHideImp();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e);
//                 }
//                 await SetVisiable(false);
//                 gameObject.SetActive(false);
//             }
//
//             await PreShowWindow();
//
//             await ShowSelfImp(args);
//
//             transform.localPosition = Vector3.zero;
//         }
//         catch (Exception e)
//         {
//             FrameworkLog.Error("show wnd failed 0!{0}", e);
//         }
//     }
//     private async Task PreShowWindow()
//     {
//         if (wndInfo.IsMainWnd)
//         {
//             if (CurShowMainUI != null && CurShowMainUI != this)
//             {
//                 Push(CurShowMainUI);
//             }
//             await ShowMainWnd(this, false);
//         }
//
//         if (!wndInfo.IsMainWnd && wndInfo.GroupId > 0)
//         {
//             if (CurShowMainUI != null && wndInfo.GroupId == CurShowMainUI.wndInfo.GroupId)
//             {
//                 if (!CurShowMainUI._childrens.Contains(this))
//                     CurShowMainUI._childrens.Add(this);
//             }
//         }
//
//         if (!_ShowedUIBhvrList.ContainsKey(this.GetType()))
//         {
//             _ShowedUIBhvrList.Add(this.GetType(), this);
//         }
//     }
//
//     private async Task ShowSelfImp(params object[] args)
//     {
//         try
//         {
//             await OnPreShowAsync(args);
//
//             if (PreShowUiHandler != null)
//             {
//                 await PreShowUiHandler(this);
//             }
//             FrameworkLog.Info("show wnd");
//             OnShowImp(args);
//             await OnShowImpAsync(args);
//         }
//         catch (Exception e)
//         {
//             FrameworkLog.Error("show wnd failed 1!{0}", e);
//         }
//
//         if (!isVisable)
//         {
//             await SetVisiable(true);
//         }
//
//         if (wndInfo.IsBlurBG == false)
//         {
//             if (!gameObject.activeSelf)
//                 gameObject.SetActive(true);
//         }
//
//         if (PostShowUiHandler != null)
//             PostShowUiHandler(this, args);
//
//         BroadcastOnShow();
//     }
//
//     public void BroadcastOnShow()
//     {
//         gameObject.BroadcastMessage<UIMonoBehaviour>("OnShow");
//         gameObject.BroadcastMessage<UIBehavior>("OnShow");
//
//     }
//
//     public void BroadcastOnHide()
//     {
//         gameObject.BroadcastMessage<UIMonoBehaviour>("OnHide");
//         gameObject.BroadcastMessage<UIBehavior>("OnHide");
//     }
//
//     public void Hide()
//     {
//         try
//         {
//             Debug.Log("Hide:" + GetType());
//             if (IsShowing)
//             {
//                 try
//                 {
//                     OnHideImp();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e);
//                 }
//                 HideSelfImpl();
//                 isVisable = false;
//
//                 if (wndInfo != null && wndInfo.IsMainWnd)
//                 {
//                     UIBehavior pop_main = Pop(this);
//
//                     if (pop_main != null && CurShowMainUI != pop_main)
//                     {
//                         UIBehaviorLog.Info("==================Hide:" + this.GetType() + " and PopShow:" + CurShowMainUI.GetType());
//
//                         CurShowMainUI = null;
//
//                         TaskUtil.Run(async () =>
//                         {
//                             await ShowMainWnd(pop_main, true);
//                             await pop_main.SetVisiable(true);
//                         });
//                     }
//                     else if (pop_main == null)
//                     {
//                         CurShowMainUI = null;
//                     }
//                 }
//             }
//             else if (this.gameObject.activeSelf)
//             {
//                 try
//                 {
//                     OnHideImp();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e);
//                 }
//                 HideSelfImpl();
//             }
//
//             if (wndInfo.DestroyOnHide)
//             {
//                 _ShowedUIBhvrList.Remove(this.GetType());
//                 DestroyImp();
//             }
//         }
//         catch (Exception e)
//         {
//             FrameworkLog.Error("hide wnd failed 0! {0}", e);
//         }
//     }
//
//     private void HideSelfImpl()
//     {
//         if (HideUiHandler != null)
//             HideUiHandler(this);
//
//         BroadcastOnHide();
//
//         _childrens.Clear();
//
//         if (wndInfo.GroupId > 0)
//         {
//             if (CurShowMainUI != null && wndInfo.GroupId == CurShowMainUI.wndInfo.GroupId)
//                 CurShowMainUI._childrens.Remove(this);
//         }
//
//         gameObject.SetActive(false);
//         _ShowedUIBhvrList.Remove(this.GetType());
//     }
//
//     virtual protected bool CanCloseThisUI()
//     {
//         return true;
//     }
//
//     private static Dictionary<Type, UIBehavior> _ShowedUIBhvrList = new Dictionary<Type, UIBehavior>();
//
//     private static int UIBehaviorSortFunc(UIBehavior a, UIBehavior b)
//     {
//         return a.sortingOrder - b.sortingOrder;
//     }
//
//     public static void HidePopWnd()
//     {
//         var all = GetAllShowedBhvr();
//         for (int i = all.Count - 1; i >= 0; i--)
//         {
//             var bhvr = all[i];
//             if (bhvr.wndInfo.Layer == 2)
//             {
//                 bhvr.Hide();
//             }
//             if (bhvr.wndInfo.Layer == 1)
//             {
//                 break;
//             }
//         }
//     }
//
//     public static void HideToTargetUI(string uiName)
//     {
//         var all = GetAllShowedBhvr();
//         for (int i = all.Count - 1; i >= 0; i--)
//         {
//             var bhvr = all[i];
//             if (bhvr.wndInfo.Layer == 1)
//             {
//                 if (bhvr.wndInfo.Name != uiName)
//                 {
//                     bhvr.Hide();
//                 }
//                 else
//                 {
//                     break;
//                 }
//             }
//
//         }
//     }
//
//     public static List<UIBehavior> GetAllShowedBhvr()
//     {
//         var list = new List<UIBehavior>(_ShowedUIBhvrList.Values);
//         var keys = new List<Type>(_ShowedUIBhvrList.Keys);
//         for (int i = list.Count - 1; i >= 0; i--)
//         {
//             if (list[i] == null)
//             {
//                 list.RemoveAt(i);
//                 var t = keys[i];
//                 _ShowedUIBhvrList.Remove(t);
//                 Debug.LogWarning($"UIBehaviour Type : {t} is NULL ! ! !");
//             }
//         }
//         list.Sort(UIBehaviorSortFunc);
//         return list;
//     }
//
//     public static bool DealGoHome { get; set; }
//     /// <summary>
//     /// 返回主界面（是否发送主界面打开事件）
//     /// </summary>
//     /// <param name="showHomeEvent"></param>
//     public static void GoHome(bool showHomeEvent = true)
//     {
//         if (CurShowMainUI != null)
//         {
//             DealGoHome = true;
//             List<UIBehavior> all = GetAllShowedBhvr();
//             while (all.Count > 0)
//             {
//                 int index = all.Count - 1;
//                 UIBehavior bhvr = all[index];
//                 if (bhvr == CurShowMainUI)
//                 {
//                     if (showHomeEvent)
//                     {
//                         if (bhvr is IVisibleHandler)
//                             (bhvr as IVisibleHandler).OnVisible(false);
//                         bhvr.BroadcastOnShow();
//                     }
//
//                     TaskUtil.Run(bhvr.SetVisiable(true));
//                     break;
//                 }
//                 bhvr.Hide();
//                 all.RemoveAt(index);
//             }
//             DealGoHome = false;
//         }
//     }
//     static public async Task ShowMainWnd(UIBehavior mainwnd, bool recovery)
//     {
//         UIBehavior oldMainUI = CurShowMainUI;
//
//         RemoveFromUIStack(mainwnd);
//         CurShowMainUI = mainwnd;
//
//         List<UIBehavior> showing = new List<UIBehavior>();
//         if (mainwnd != null)
//         {
//             showing.Add(mainwnd);
//         }
//         List<UIBehavior> all = new List<UIBehavior>(_ShowedUIBhvrList.Values);
//         while (all.Count > 0)
//         {
//             int index = all.Count - 1;
//             UIBehavior bhvr = all[index];
//             all.RemoveAt(index);
//             if (!bhvr.wndInfo.CanHideByMainWnd)
//             {
//                 showing.Add(bhvr);
//                 continue;
//             }
//             if (!bhvr.CanCloseThisUI())
//             {
//                 showing.Add(bhvr);
//                 continue;
//             }
//             if (bhvr != null && bhvr != mainwnd)
//             {
//                 if (!bhvr.isVisable)
//                 {
//                     showing.Add(bhvr);
//                 }
//                 else if (bhvr.wndInfo.IsMainWnd)
//                 {
//                     await bhvr.SetVisiable(false);
//                     showing.Add(bhvr);
//                 }
//                 else if (oldMainUI != null && oldMainUI._childrens.Contains(bhvr))
//                 {
//                     await bhvr.SetVisiable(false);
//                     showing.Add(bhvr);
//                 }
//                 else if (mainwnd._childrens.Contains(bhvr))
//                 {
//                     showing.Add(bhvr);
//                 }
//                 else if (mainwnd.wndInfo.GroupId != 0 && bhvr.wndInfo.GroupId == mainwnd.wndInfo.GroupId)
//                 {
//                     mainwnd._childrens.Add(bhvr);
//                     showing.Add(bhvr);
//                 }
//                 else
//                 {
//                     FrameworkLog.Info(bhvr.name + " not handled");
//                 }
//             }
//         }
//         _ShowedUIBhvrList.Clear();
//         foreach (UIBehavior bhvr in showing)
//         {
//             _ShowedUIBhvrList.Add(bhvr.GetType(), bhvr);
//         }
//         foreach (UIBehavior child in mainwnd._childrens)
//         {
//             await child.SetVisiable(true);
//         }
//
//         if (ShowMainWndHandler != null)
//             ShowMainWndHandler(mainwnd);
//     }
//
//     static public void CloseAll(bool force)
//     {
//         foreach (UIBehavior bhvr in uiStack)
//         {
//             try
//             {
//                 bhvr.HideSelfImpl();
//                 bhvr.OnHideImp();
//             }
//             catch (Exception e)
//             {
//                 FrameworkLog.Error(e.ToString());
//             }
//         }
//         uiStack.Clear();
//
//         List<UIBehavior> all = new List<UIBehavior>(_ShowedUIBhvrList.Values);
//         while (all.Count > 0)
//         {
//             int index = all.Count - 1;
//             UIBehavior bhvr = all[index];
//             all.RemoveAt(index);
//
//             try
//             {
//                 bhvr.HideSelfImpl();
//                 bhvr.OnHideImp();
//             }
//             catch (Exception e)
//             {
//                 FrameworkLog.Error(e.ToString());
//             }
//
//             if (force || bhvr.wndInfo.DestroyOnChangeScene)
//                 bhvr.DestroyImp();
//         }
//         _ShowedUIBhvrList.Clear();
//
//         List<UIBehavior> tmp = new List<UIBehavior>(uiBehaviourDict.Values);
//         foreach (var item in tmp)
//         {
//             if (force || item.wndInfo.DestroyOnChangeScene)
//             {
//                 item.DestroyImp();
//             }
//         }
//     }
//
//     static Stack<UIBehavior> uiStack = new Stack<UIBehavior>();
//     static UIBehavior CurShowMainUI = null;
//
//     static public UIBehavior GetCurShowMainUI()
//     {
//         return CurShowMainUI;
//     }
//
//     static public Dictionary<Type, UIBehavior> GetShowUIBehaviours()
//     {
//         return _ShowedUIBhvrList;
//     }
//
//     static public bool IsMainUIVisible(Type p)
//     {
//         if (CurShowMainUI == null || CurShowMainUI.GetType() != p)
//         {
//             return false;
//         }
//         return CurShowMainUI.IsVisible;
//     }
//
//     static public bool IsThisTypeUIShow(Type p)
//     {
//         return _ShowedUIBhvrList.ContainsKey(p) ? true : false;
//     }
//
//     static public UIBehavior GetThisTypeUIShow(Type p)
//     {
//         return _ShowedUIBhvrList.ContainsKey(p) ? _ShowedUIBhvrList[p] : null;
//     }
//
//     static void Push(UIBehavior bhvr)
//     {
//         uiStack.Push(bhvr);
//     }
//
//     static void RemoveFromUIStack(UIBehavior bhvr)
//     {
//         if (uiStack.Contains(bhvr))
//         {
//             List<UIBehavior> tmp = new List<UIBehavior>();
//             while (true)
//             {
//                 UIBehavior top = uiStack.Pop();
//                 if (top == bhvr)
//                 {
//                     break;
//                 }
//                 tmp.Add(top);
//             }
//             for (int i = tmp.Count - 1; i >= 0; --i)
//             {
//                 uiStack.Push(tmp[i]);
//             }
//         }
//     }
//
//     static UIBehavior Pop(UIBehavior bhvr)
//     {
//         if (CurShowMainUI == bhvr)
//         {
//             if (uiStack.Count > 0)
//             {
//                 return uiStack.Pop();
//             }
//             else
//             {
//                 return null;
//             }
//         }
//         else
//         {
//             RemoveFromUIStack(bhvr);
//
//             UIBehaviorLog.Info("Pop CurShowMainUI=" + ((CurShowMainUI != null) ? CurShowMainUI.ToString() : "null") + " bhvr=" + bhvr + " uiStack.Count=" + uiStack.Count);
//             return CurShowMainUI;
//         }
//     }
//
//     public static void ClearUIStack()
//     {
//         try
//         {
//             foreach (UIBehavior bhvr in uiStack)
//             {
//                 try
//                 {
//                     bhvr.HideSelfImpl();
//                     bhvr.OnHideImp();
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e);
//                 }
//             }
//             uiStack.Clear();
//
//             UIBehavior except = null;
//             List<UIBehavior> all = new List<UIBehavior>(_ShowedUIBhvrList.Values);
//             while (all.Count > 0)
//             {
//                 int index = all.Count - 1;
//                 UIBehavior bhvr = all[index];
//                 all.RemoveAt(index);
//
//                 if (bhvr.wndInfo.HideOnChangeScene)
//                 {
//                     try
//                     {
//                         bhvr.HideSelfImpl();
//                         bhvr.OnHideImp();
//                     }
//                     catch (Exception e)
//                     {
//                         Debug.LogError(e);
//                     }
//
//                     if (bhvr.wndInfo.DestroyOnChangeScene)
//                         bhvr.DestroyImp();
//                 }
//                 else
//                 {
//                     except = bhvr;
//                 }
//             }
//             _ShowedUIBhvrList.Clear();
//             if (except != null)
//                 _ShowedUIBhvrList.Add(except.GetType(), except);
//
//             List<UIBehavior> tmp = new List<UIBehavior>(uiBehaviourDict.Values);
//             foreach (var item in tmp)
//             {
//                 if (item.wndInfo.DestroyOnChangeScene)
//                 {
//                     item.DestroyImp();
//                 }
//             }
//
//             CurShowMainUI = null;
//         }
//         catch (Exception e)
//         {
//             FrameworkLog.Error("ClearUIStack failed!{0}", e);
//         }
//     }
//
//     #endregion
//
//     #region UI资源管理
//     private static Dictionary<string, UIBehavior> uiBehaviourDict = new Dictionary<string, UIBehavior>();
//     private static Dictionary<string, GameObject> loadedUIPrefabs = new Dictionary<string, GameObject>();
//
//     public static void ShowUIByName(string name, params object[] args)
//     {
//         TaskUtil.Run(ShowUIByNameImp(name, args));
//     }
//     public static void ShowUI<T>() where T : UIBehavior
//     {
//         TaskUtil.Run(ShowUIByNameImp(typeof(T).Name));
//     }
//     public static void ShowUI<T>(params object[] args) where T : UIBehavior
//     {
//         TaskUtil.Run(ShowUIByNameImp(typeof(T).Name, args));
//     }
//     public static Task ShowUIByNameAsync(string name, params object[] args)
//     {
//         return ShowUIByNameImp(name, args);
//     }
//     public static Task ShowUIAsync<T>() where T : UIBehavior
//     {
//         return ShowUIByNameImp(typeof(T).Name);
//     }
//     public static Task ShowUIAsync<T>(params object[] args) where T : UIBehavior
//     {
//         return ShowUIByNameImp(typeof(T).Name, args);
//     }
//     protected static Task ShowUIByNameImp(string name, params object[] args)
//     {
//         Debug.Log($"ShowUIByNameImp:{name}");
//         var tcs = new TaskCompletionSource<bool>();
//         void showUIBehaviour(UIBehavior bhvr)
//         {
//             if (UIBehavior.LoadSceneHandler != null)
//             {
//                 UIBehavior.LoadSceneHandler(bhvr, () =>
//                 {
//                     TaskUtil.Run(async () =>
//                     {
//                         await bhvr.ShowImp(args);
//                         tcs.SetResult(true);
//                     });
//                 });
//             }
//             else
//             {
//                 TaskUtil.Run(async () =>
//                 {
//                     await bhvr.ShowImp(args);
//                     tcs.SetResult(true);
//                 });
//             }
//         }
//         UIBehavior uiBehaviour = null;
//         if (uiBehaviourDict.TryGetValue(name, out uiBehaviour))
//         {
//             showUIBehaviour(uiBehaviour);
//         }
//         else
//         {
//             LoadUI(name, (behaviour) =>
//             {
//                 showUIBehaviour(behaviour);
//             });
//         }
//         return tcs.Task;
//     }
//
//     public static void HideUI(string name, bool force = false)
//     {
//         UIBehavior uiBehaviour = null;
//         if (uiBehaviourDict.TryGetValue(name, out uiBehaviour))
//         {
//             uiBehaviour.Hide();
//         }
//         else
//         {
//             FrameworkLog.Warn($"HideUI {name} not found!");
//         }
//     }
//     public static void HideUI<T>() where T : UIBehavior
//     {
//         HideUI(typeof(T).Name, false);
//     }
//     public static void HideUI<T>(bool force = false) where T : UIBehavior
//     {
//         HideUI(typeof(T).Name, force);
//     }
//
//     public static bool IsShow(string name)
//     {
//         UIBehavior uiBehaviour = null;
//         if (uiBehaviourDict.TryGetValue(name, out uiBehaviour))
//         {
//             return uiBehaviour.IsVisible;
//         }
//         else
//         {
//             return false;
//         }
//     }
//
//     public static bool IsExist(string name)
//     {
//         return uiBehaviourDict.ContainsKey(name);
//     }
//     public static bool IsExist<T>() where T : UIBehavior
//     {
//         return IsExist(typeof(T).Name);
//     }
//     public static bool IsShow<T>() where T : UIBehavior
//     {
//         return IsShow(typeof(T).Name);
//     }
//
//     static public void LoadUI(string name, Action<UIBehavior> ret)
//     {
//         if (name != "DisableOperationUIBehavior")
//             EventManager.DispatchEvent(new E_DisableUIOperation());
//         GetUIPrefabByUIBehaviourName(name, (prefab, wndinfo) =>
//         {
//             if (name != "DisableOperationUIBehavior")
//                 EventManager.DispatchEvent(new E_EnableUIOperation());
//
//             if (prefab == null)
//             {
//                 UIBehaviorLog.Error("! ! ! Can't find in UI prefab. " + name);
//                 return;
//             }
//
//             UIBehavior uiBehaviour = GetUIBehaviour(name, prefab);
//             if (uiBehaviour == null)
//             {
//                 UIBehaviorLog.Error("! ! ! Can't UIBehavior find in UI prefab. " + name);
//                 return;
//             }
//             if (wndinfo == null)
//             {
//                 wndinfo = Table_UIWnd.getByName(name);
//                 if (wndinfo == null)
//                 {
//                     UIBehaviorLog.Warn("! ! ! Can't find in UI config. " + name);
//                     return;
//                 }
//             }
//
//             uiBehaviour.wndInfo = wndinfo;
//
//             if (LoadUiHandler != null)
//                 LoadUiHandler(uiBehaviour);
//
//             //uiBehaviour.gameObject.SetActive(false);
//
//             if (ret != null)
//                 ret(uiBehaviour);
//         });
//     }
//     static public void LoadUI<T>(Action<T> ret) where T : UIBehavior
//     {
//         LoadUI(typeof(T).Name, (baseBehavior) =>
//         {
//             if (ret != null)
//                 ret((T)baseBehavior);
//         });
//     }
//
//     void DestroyImp()
//     {
//         string uibehaviourName = wndInfo.Name;
//         if (loadedUIPrefabs.ContainsKey(uibehaviourName))
//         {
//             UnityEngine.Object.Destroy(loadedUIPrefabs[uibehaviourName]);
//             loadedUIPrefabs.Remove(uibehaviourName);
//
//             UIBehaviorLog.Info("-----Destroy Prefab:" + uibehaviourName);
//         }
//         else
//         {
//             UIBehaviorLog.Info("=====DestroyUI Can't find uibehaviourName:" + uibehaviourName);
//         }
//
//         uiBehaviourDict.Remove(uibehaviourName);
//
//         Destroy(gameObject);
//     }
//
//     static public UIBehavior GetUIBehaviour(string name, GameObject __uiGO = null)
//     {
//         UIBehavior uiBehaviour = null;
//         if (uiBehaviourDict.TryGetValue(name, out uiBehaviour))
//         {
//             return uiBehaviour;
//         }
//
//         if (__uiGO)
//         {
//             uiBehaviour = __uiGO.GetComponent<UIBehavior>(true);
//             if (uiBehaviour == null)
//             {
//                 var monos = __uiGO.GetComponentsInChildren<UIBehavior>(true);
//                 if (monos.Length > 0)
//                     uiBehaviour = monos[0];
//             }
//             if (uiBehaviour == null)
//             {
//                 UIBehaviorLog.Error("error ：GetUIBehaviour cant find " + name + " UIBehavior");
//             }
//         }
//
//         if (uiBehaviour != null)
//         {
//             if (!uiBehaviourDict.ContainsKey(name))
//             {
//                 uiBehaviourDict.Add(name, uiBehaviour);
//             }
//         }
//
//         return uiBehaviour;
//     }
//     static public T GetUIBehaviour<T>(GameObject __uiGO = null) where T : UIBehavior
//     {
//         return (T)GetUIBehaviour(typeof(T).Name, __uiGO);
//     }
//
//     static private void GetUIPrefabByUIBehaviourName(string uibehaviourName, Action<GameObject, Table_UIWnd> callback)
//     {
//         GameObject returnValue;
//         if (loadedUIPrefabs.TryGetValue(uibehaviourName, out returnValue))
//         {
//             callback(returnValue, null);
//             return;
//         }
//         Table_UIWnd wndinfo = Table_UIWnd.getByName(uibehaviourName);
//         if (wndinfo == null)
//         {
//             UIBehaviorLog.Error("! ! ! Can't find in UI config. " + uibehaviourName);
//             callback(null, null);
//             return;
//         }
//         string prefabDir = wndinfo.Path;
//         if (!string.IsNullOrEmpty(prefabDir))
//         {
//             OnBeforeLoadPrefab?.Invoke(wndinfo);
//             AssetLoader.instance.LoadAsset<GameObject>(prefabDir, (obj) =>
//             {
//                 if (obj == null)
//                 {
//                     UIBehaviorLog.Error("! ! ! Can't find UI Prefab ! ! ! prefabDir：" + prefabDir);
//                     callback(null, null);
//                     return;
//                 }
//                 if (loadedUIPrefabs.ContainsKey(uibehaviourName))
//                     return;
//                 OnLoadPrefb?.Invoke(obj);
//                 GameObject go = GameObject.Instantiate(obj) as GameObject;
//                 go.transform.SetParent(WindowRoot.transform);
//                 go.transform.Reset();
//                 loadedUIPrefabs.Add(uibehaviourName, go);
//                 UIBehaviorLog.Info("-----Add Prefab:" + uibehaviourName);
//                 callback(go, wndinfo);
//             });
//             return;
//         }
//
//         UIBehaviorLog.Error("  error : GetUIPrefabByUIBehaviourName  cant find uibehaviour:" + uibehaviourName + " prefabDir:" + prefabDir);
//
//         callback(null, null);
//     }
//     #endregion 
// }