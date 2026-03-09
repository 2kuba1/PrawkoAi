import { Stack, useRouter } from "expo-router";
import { createContext, useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import "./globals.css";

export const AuthContext = createContext({
  signIn: () => {},
});

export default function RootLayout() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const router = useRouter();

  const signIn = () => {
    setIsLoggedIn(true);
    router.replace("/dashboard");
  };

  return (
    <AuthContext.Provider value={{ signIn }}>
      <SafeAreaProvider>
        <Stack screenOptions={{ headerShown: false }}>
          <Stack.Screen name="index" />
          <Stack.Screen name="dashboard" />
        </Stack>
      </SafeAreaProvider>
    </AuthContext.Provider>
  );
}
