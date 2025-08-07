import axios from 'axios';

const instance = axios.create({
  baseURL: import.meta.env.VITE_BACKEND_API,
  withCredentials: true,
});

instance.interceptors.request.use(
  (config) => {
    const token = document.cookie
      .split('; ')
      .find((row) => row.startsWith('VAcademyConnect='))
      ?.split('=')[1];

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

instance.interceptors.response.use(
  (response) => response,
  async (error) => {
    switch (error.response?.status) {
      default:
        return Promise.reject(error);
    }
  }
);

export default instance;
