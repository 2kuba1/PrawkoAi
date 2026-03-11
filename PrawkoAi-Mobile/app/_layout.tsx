import { Stack, useRouter, useSegments } from "expo-router";
import * as SecureStore from "expo-secure-store";
import { jwtDecode } from "jwt-decode";
import { createContext, useEffect, useState } from "react";
import {
  configureReanimatedLogger,
  ReanimatedLogLevel,
} from "react-native-reanimated";
import { SafeAreaProvider } from "react-native-safe-area-context";
import "./globals.css";

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

    const isAuthPage =
      rootSegment === "dashboard" || rootSegment === "examRulesScreen";

    if (!token && isAuthPage) {
      router.replace("/");
    } else if (token && (rootSegment === "index" || !rootSegment)) {
      router.replace("/dashboard");
    }
  }, [token, isLoading, segments]);

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
        const apiUrl = `${process.env.EXPO_PUBLIC_API_URL}/api/account/logout`;

        await fetch(apiUrl, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token?.accessToken}`,
            "Content-Type": "application/json",
          },
        });
      } catch (e) {
        console.log(e);
      } finally {
        await SecureStore.deleteItemAsync("userToken");
        setToken(null);
      }
    },
  };

  return (
    <AuthContext.Provider value={authContextValue}>
      <SafeAreaProvider>
        <Stack screenOptions={{ headerShown: false }}>
          <Stack.Screen name="index" />
          <Stack.Screen name="dashboard" />
          <Stack.Screen name="examRules" />
          <Stack.Screen name="examSimulation" />
        </Stack>
      </SafeAreaProvider>
    </AuthContext.Provider>
  );
}
