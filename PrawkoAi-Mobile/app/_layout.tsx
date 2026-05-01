import AsyncStorage from "@react-native-async-storage/async-storage";
import { getLocales } from "expo-localization";
import * as Notifications from "expo-notifications";
import { Stack, useRouter, useSegments } from "expo-router";
import * as SecureStore from "expo-secure-store";
import { jwtDecode } from "jwt-decode";
import { useColorScheme } from "nativewind";
import { createContext, useEffect, useState } from "react";
import { AppState, AppStateStatus } from "react-native";
import {
  configureReanimatedLogger,
  ReanimatedLogLevel,
} from "react-native-reanimated";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { ErrorProvider } from "./context/errorContext";
import "./globals.css";
import api from "./utils/api";
import i18n from "./utils/translations";

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
      seconds: 3600,
    },
  });
}

export default function RootLayout() {
  const [token, setToken] = useState<TokenResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [user, setUser] = useState<User | null>(null);
  const router = useRouter();
  const segments = useSegments();

  const { setColorScheme } = useColorScheme();
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const loadAppSettings = async () => {
      try {
        const [savedTheme, savedLang, savedCat] = await Promise.all([
          AsyncStorage.getItem("user-theme"),
          AsyncStorage.getItem("user-language"),
          AsyncStorage.getItem("user-category"),
        ]);

        if (savedTheme) {
          setColorScheme(savedTheme as "light" | "dark");
        }

        if (savedLang) {
          i18n.locale = savedLang;
        } else {
          const locale = getLocales()[0];
          const code = locale
            ? (
                locale.languageCode || locale.languageTag.split("-")[0]
              ).toUpperCase()
            : "EN";
          i18n.locale = code;
        }
      } catch (e) {
        console.error("Błąd podczas ładowania ustawień w RootLayout:", e);
      } finally {
        setIsReady(true);
      }
    };

    loadAppSettings();
  }, []);

  const decodeAndSetUser = (tokenStr: string) => {
    try {
      const decoded = jwtDecode(tokenStr) as User;
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
    if (isLoading || !isReady) return;

    const currentSegments = segments as string[];
    const rootSegment = currentSegments[0];

    const isLoginPage = rootSegment === "index" || rootSegment === undefined;
    if (!token && !isLoginPage) {
      router.replace("/");
    } else if (token && isLoginPage) {
      router.replace("/dashboard");
    }
  }, [token, isLoading, isReady, segments]);

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
        await api.delete(`/account/logout?userId=${user?.id}`);
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

  if (!isReady) return null;

  return (
    <AuthContext.Provider value={authContextValue}>
      <ErrorProvider>
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
            <Stack.Screen
              name="exam/examSimulation"
              options={{
                gestureEnabled: false,
                headerLeft: () => null,
              }}
            />
            <Stack.Screen
              name="exam/examResult/[id]"
              options={{
                gestureEnabled: false,
                headerLeft: () => null,
              }}
            />
            <Stack.Screen name="question/examQuestionWithAnswer/[id]" />
            <Stack.Screen name="user/stats" />
            <Stack.Screen name="learning/studyTopics" />
            <Stack.Screen name="learning/studyTopic/[category]" />
            <Stack.Screen
              name="learning/setSolving/[category]"
              options={{
                gestureEnabled: false,
                headerLeft: () => null,
              }}
            />
            <Stack.Screen
              name="learning/setResult/setResult"
              options={{
                gestureEnabled: false,
                headerLeft: () => null,
              }}
            />
            <Stack.Screen name="user/profile" />
            <Stack.Screen name="question/questionWithAnswers/[questionNumber]" />
          </Stack>
        </SafeAreaProvider>
      </ErrorProvider>
    </AuthContext.Provider>
  );
}
