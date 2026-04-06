import * as Notifications from "expo-notifications";
import { Stack, useRouter, useSegments } from "expo-router";
import * as SecureStore from "expo-secure-store";
import { jwtDecode } from "jwt-decode";
import { createContext, useEffect, useState } from "react";
import { AppState, AppStateStatus } from "react-native";
import {
  configureReanimatedLogger,
  ReanimatedLogLevel,
} from "react-native-reanimated";
import { SafeAreaProvider } from "react-native-safe-area-context";
import "./globals.css";
import api from "./utils/api";

export interface AuthContextType {
  signIn: (token: TokenResponse) => Promise<void>;
  signOut: () => Promise<void>;
  token: TokenResponse | null;
  isLoading: boolean;
  user: User | null;
}

export interface User {
  id: string;
  email: string;
  deviceId: string;
  role: string;
}

export const AuthContext = createContext<AuthContextType>({
  signIn: async () => {},
  signOut: async () => {},
  token: null,
  isLoading: true,
  user: null,
});

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

configureReanimatedLogger({
  level: ReanimatedLogLevel.warn,
  strict: false,
});

async function scheduleInactivityNotification() {
  await Notifications.cancelAllScheduledNotificationsAsync();
  await Notifications.scheduleNotificationAsync({
    content: {
      title: "Wróć do nauki! 📚",
      body: "Twoje testy czekają. Nie pozwól, aby wiedza wyparowała!",
      sound: true,
      priority: Notifications.AndroidNotificationPriority.HIGH,
    },
    trigger: {
      type: Notifications.SchedulableTriggerInputTypes.TIME_INTERVAL,
      seconds: 30,
    },
  });
}

export default function RootLayout() {
  const [token, setToken] = useState<TokenResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState<User | null>(null);
  const router = useRouter();
  const segments = useSegments();

  const decodeAndSetUser = (token: string) => {
    try {
      const decoded = jwtDecode(token) as User;
      setUser(decoded);
    } catch (e) {
      setUser(null);
    }
  };

  useEffect(() => {
    async function initAuth() {
      try {
        const savedToken = await SecureStore.getItemAsync("userToken");
        if (savedToken) {
          const parsed = JSON.parse(savedToken) as TokenResponse;

          if (parsed?.accessToken) {
            setToken(parsed);
            decodeAndSetUser(parsed.accessToken);
          }
        }
      } catch (e) {
      } finally {
        setIsLoading(false);
      }
    }
    initAuth();
  }, []);

  useEffect(() => {
    if (isLoading) return;

    const currentSegments = segments as string[];
    const rootSegment = currentSegments[0];

    const isLoginPage = rootSegment === "index" || rootSegment === undefined;
    if (!token && !isLoginPage) {
      router.replace("/");
    } else if (token && isLoginPage) {
      router.replace("/dashboard");
    }
  }, [token, isLoading, segments]);

  useEffect(() => {
    const subscription = AppState.addEventListener(
      "change",
      (state: AppStateStatus) => {
        if (state === "background") {
          scheduleInactivityNotification();
        }
      },
    );

    return () => {
      subscription.remove();
    };
  }, []);

  const authContextValue = {
    token,
    isLoading,
    user,
    signIn: async (newToken: TokenResponse) => {
      await SecureStore.setItemAsync("userToken", JSON.stringify(newToken));
      setToken(newToken);
      decodeAndSetUser(newToken.accessToken);
    },
    signOut: async () => {
      try {
        await api.delete(`/api/account/logout?userId=${user?.id}`);
      } catch (e) {
        console.log(e);
      } finally {
        await SecureStore.deleteItemAsync("userToken");
        setToken(null);
        setUser(null);
        await Notifications.cancelAllScheduledNotificationsAsync();
        router.replace("/");
      }
    },
  };

  useEffect(() => {
    const requestPermissions = async () => {
      const { status } = await Notifications.getPermissionsAsync();
      if (status !== "granted") {
        await Notifications.requestPermissionsAsync();
      }
    };

    requestPermissions();

    const handleAppStateChange = (nextAppState: AppStateStatus) => {
      if (
        token &&
        (nextAppState === "background" || nextAppState === "inactive")
      ) {
        scheduleInactivityNotification();
      }

      if (nextAppState === "active") {
        Notifications.cancelAllScheduledNotificationsAsync();
      }
    };

    const subscription = AppState.addEventListener(
      "change",
      handleAppStateChange,
    );

    return () => {
      subscription.remove();
    };
  }, [token]);

  return (
    <AuthContext.Provider value={authContextValue}>
      <SafeAreaProvider>
        <Stack
          screenOptions={{
            headerShown: false,
            animation: "fade",
            animationDuration: 200,
          }}
        >
          <Stack.Screen name="index" />
          <Stack.Screen name="dashboard" />
          <Stack.Screen name="exam/examHistory" />
          <Stack.Screen name="exam/examRules" />
          <Stack.Screen name="exam/examSimulation" />
          <Stack.Screen name="exam/examResult/[id]" />
          <Stack.Screen name="question/examQuestionWithAnswer/[id]" />
        </Stack>
      </SafeAreaProvider>
    </AuthContext.Provider>
  );
}
