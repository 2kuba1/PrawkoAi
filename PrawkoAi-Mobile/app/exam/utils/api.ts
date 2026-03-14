import { type TokenResponse } from "@/app/_layout";
import axios from "axios";
import * as SecureStore from "expo-secure-store";

const api = axios.create({
  baseURL: process.env.EXPO_PUBLIC_API_URL,
});

api.interceptors.request.use(
  async (config) => {
    const savedToken = await SecureStore.getItemAsync("userToken");
    if (savedToken) {
      const { accessToken } = JSON.parse(savedToken);
      config.headers.Authorization = `Bearer ${accessToken}`;
      config.headers["Content-Type"] = "application/json";
    }
    return config;
  },
  (error) => Promise.reject(error),
);

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const savedTokenString = await SecureStore.getItemAsync("userToken");
        if (!savedTokenString) throw new Error("No refresh token available");

        const { refreshToken }: TokenResponse = JSON.parse(savedTokenString);

        const response = await axios.get(
          `${process.env.EXPO_PUBLIC_API_URL}/api/account/refresh-token?refreshToken=${encodeURIComponent(refreshToken)}`,
        );

        const { accessToken: newAccess, refreshToken: newRefresh } =
          response.data;

        await SecureStore.setItemAsync(
          "userToken",
          JSON.stringify({ accessToken: newAccess, refreshToken: newRefresh }),
        );

        originalRequest.headers.Authorization = `Bearer ${newAccess}`;

        return api(originalRequest);
      } catch (refreshError) {
        await SecureStore.deleteItemAsync("userToken");
        return Promise.reject(refreshError);
      }
    }
    return Promise.reject(error);
  },
);

export default api;
