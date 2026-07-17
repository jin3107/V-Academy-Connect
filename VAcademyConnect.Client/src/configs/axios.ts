import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5177", // cùng scheme http với FE để cookie AuthToken (SameSite=Lax) được gửi kèm
  withCredentials: true, // Quan trọng: cho phép gửi cookies với mỗi request
});

// Request interceptor - cookies sẽ tự động được gửi
api.interceptors.request.use(
  (config) => {
    // Không cần lấy token từ localStorage nữa
    // Token sẽ được lưu trong httpOnly cookie và tự động gửi đi
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor xử lý lỗi
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Nếu bị 401 (Unauthorized), redirect về login
    // Bỏ qua /accounts/me: 401 ở đây chỉ có nghĩa là chưa đăng nhập, không phải phiên hết hạn
    if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url?.includes('/accounts/me')) {
      originalRequest._retry = true;

      // Xóa thông tin auth và redirect về login
      // Cookie sẽ được backend xóa
      window.location.href = "/login";
      return Promise.reject(error);
    }

    // Nếu bị 403 (Forbidden), redirect về unauthorized
    if (error.response?.status === 403) {
      window.location.href = "/unauthorized";
    }

    return Promise.reject(error);
  }
);

export default api;
