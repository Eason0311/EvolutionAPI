namespace prjEvolutionAPI.Services
{
    public class ServiceResult<T>
    {
        /// <summary>此 Service 是否執行成功</summary>
        public bool IsSuccess { get; private set; }

        /// <summary>失敗時的錯誤訊息，成功時為 null</summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>成功時回傳的資料，失敗時為 default(T)</summary>
        public T? Data { get; private set; }

        /// <summary>私有建構子，透過靜態方法建立</summary>
        private ServiceResult(bool isSuccess, T? data = default, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        /// <summary>建立一個代表「成功」的結果，並附上資料</summary>
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>(isSuccess: true, data: data);
        }

        /// <summary>建立一個代表「失敗」的結果，並附上錯誤訊息</summary>
        public static ServiceResult<T> Fail(string errorMessage)
        {
            return new ServiceResult<T>(isSuccess: false, errorMessage: errorMessage);
        }
    }
}
