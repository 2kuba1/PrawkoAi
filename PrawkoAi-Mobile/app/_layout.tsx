import { Stack, useRouter, useSegments } from "expo-router";
import * as SecureStore from "expo-secure-store";
import { createContext, useEffect, useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import "./globals.css";

interface AuthContextType {
  signIn: (token: string) => Promise<void>;
  signOut: () => Promise<void>;
  token: string | null;
  isLoading: boolean;
}

export const AuthContext = createContext<AuthContextType>({
  signIn: async () => {},
  signOut: async () => {},
  token: null,
  isLoading: true,
});

export default function RootLayout() {
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();
  const segments = useSegments();

  useEffect(() => {
    async function initAuth() {
      try {
        const savedToken = await SecureStore.getItemAsync("userToken");
        if (savedToken) setToken(savedToken);
      } catch (e) {
      } finally {
        setIsLoading(false);
      }
    }
    initAuth();
  }, []);

  useEffect(() => {
    if (isLoading) return;
    const inAuthGroup = segments[0] === "dashboard";

    if (!token && inAuthGroup) {
      router.replace("/");
    } else if (token && !inAuthGroup) {
      router.replace("/dashboard");
    }
  }, [token, isLoading, segments]);

  const authContextValue = {
    token,
    isLoading,
    signIn: async (newToken: string) => {
      await SecureStore.setItemAsync("userToken", newToken);
      setToken(newToken);
    },
    signOut: async () => {
      console.log("Rozpoczynam wylogowanie...");
      try {
        const apiUrl =
          "https://commander-settle-reviewer-planners.trycloudflare.com/api/account/logout";

        await fetch(apiUrl, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
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
        </Stack>
      </SafeAreaProvider>
    </AuthContext.Provider>
  );
}
