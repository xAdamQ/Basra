using Basra.Client;
using UnityEngine;

namespace Basra.Client
{
    public interface IAppInterface
    {
        public UUILoadingFeedback LoadingFeedback { get; }
    }

    public class AppInterface : MonoBehaviour, IAppInterface
    {
        [SerializeField] private UUILoadingFeedback loadingFeedback;

        public UUILoadingFeedback LoadingFeedback
        {
            get => loadingFeedback;
        }
    }
}