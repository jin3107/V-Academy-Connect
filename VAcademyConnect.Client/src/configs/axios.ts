import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5177",
  withCredentials: true,
});

api.interceptors.request.use(
  (config) => {
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry && !originalRequest.url?.includes('/accounts/me')) {
      originalRequest._retry = true;

      window.location.href = "/login";
      return Promise.reject(error);
    }

    if (error.response?.status === 403) {
      window.location.href = "/unauthorized";
    }

    return Promise.reject(error);
  }
);

export default api;
