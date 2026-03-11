import { FontAwesome5, MaterialIcons } from "@expo/vector-icons";
import * as AuthSession from "expo-auth-session";
import * as Device from "expo-device";
import * as WebBrowser from "expo-web-browser";
import React, { useContext } from "react";
import { StatusBar, Text, TouchableOpacity, View } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "./_layout";

WebBrowser.maybeCompleteAuthSession();

export default function LoginScreen() {
  const insets = useSafeAreaInsets();
  const { signIn: completeSignIn } = useContext(AuthContext);

  const handleGoogleSignIn = async () => {
    const redirectUri = AuthSession.makeRedirectUri({ scheme: "prawkoai" });
    const deviceId = Device.osInternalBuildId ?? "dev_id";
    const deviceName = Device.deviceName ?? "Mobile";

    const apiUrl = `${process.env.EXPO_PUBLIC_API_URL}/api/account/login/google`;
    const authUrl = `${apiUrl}?returnUrl=${encodeURIComponent(redirectUri)}&deviceId=${encodeURIComponent(deviceId)}&deviceName=${encodeURIComponent(deviceName)}`;

    try {
      const result = await WebBrowser.openAuthSessionAsync(
        authUrl,
        redirectUri,
      );
      if (result.type === "success" && result.url) {
        const url = new URL(result.url);

        const accessToken = url.searchParams.get("accessToken");
        const refreshToken = url.searchParams.get("refreshToken");

        if (accessToken && refreshToken) {
          await completeSignIn({
            accessToken,
            refreshToken,
          });
        }
      }
    } catch (error) {
      console.error("Błąd sesji przeglądarki:", error);
    }
  };

  return (
    <View
      className="flex-1 bg-[#f6f6f8] dark:bg-[#111621] items-center justify-center p-6"
      style={{ paddingTop: insets.top, paddingBottom: insets.bottom }}
    >
      <StatusBar barStyle="dark-content" />

      <View className="absolute -top-10 -right-10 w-64 h-64 bg-[#1544b2]/5 rounded-full blur-3xl" />
      <View className="absolute -bottom-20 -left-20 w-80 h-80 bg-[#1544b2]/5 rounded-full blur-3xl" />

      <View className="w-full max-w-sm items-center">
        <View className="mb-10 w-32 h-32 items-center justify-center bg-white dark:bg-slate-900 rounded-3xl shadow-xl shadow-blue-900/10 border border-slate-100 dark:border-slate-800 overflow-hidden">
          <MaterialIcons name="auto-fix-high" size={60} color="#1544b2" />
        </View>

        <Text className="text-slate-900 dark:text-white tracking-tight text-3xl font-bold text-center mb-3">
          Witaj w Prawko AI
        </Text>
        <Text className="text-slate-600 dark:text-slate-400 text-lg text-center leading-6 mb-12 px-4">
          Zacznij naukę na prawo jazdy z pomocą AI
        </Text>

        <View className="w-full gap-4">
          <TouchableOpacity
            onPress={handleGoogleSignIn}
            activeOpacity={0.8}
            className="flex-row w-full items-center justify-center rounded-2xl h-16 bg-[#1544b2] px-6 gap-3 shadow-lg shadow-[#1544b2]/30"
          >
            <FontAwesome5 name="google" size={20} color="white" />
            <Text className="text-white text-base font-bold tracking-wide">
              Zaloguj przez Google
            </Text>
          </TouchableOpacity>

          <TouchableOpacity
            activeOpacity={0.7}
            className="flex-row w-full items-center justify-center rounded-2xl h-16 bg-[#1544b2]/10 dark:bg-[#1544b2]/20 px-6 gap-2"
          >
            <MaterialIcons name="person-outline" size={22} color="#1544b2" />
            <Text className="text-[#1544b2] dark:text-blue-400 text-base font-semibold">
              Kontynuuj jako gość
            </Text>
          </TouchableOpacity>
        </View>

        <View className="mt-16 pt-8 border-t border-slate-200 dark:border-slate-800 w-full items-center">
          <Text className="text-[10px] text-slate-400 dark:text-slate-500 mb-6 uppercase tracking-[3px] font-bold">
            Inteligentny Asystent Kierowcy
          </Text>

          <View className="flex-row gap-3">
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/40" />
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/20" />
            <View className="w-2 h-2 rounded-full bg-[#1544b2]/10" />
          </View>
        </View>
      </View>
    </View>
  );
}
