import { MaterialCommunityIcons } from "@expo/vector-icons";
import React, {
  createContext,
  FC,
  ReactNode,
  useCallback,
  useContext,
  useState,
} from "react";
import { Pressable, Text, View } from "react-native";
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withSpring,
  withTiming,
} from "react-native-reanimated";
import { SafeAreaView } from "react-native-safe-area-context";

type ErrorContextType = {
  showError: (message: string) => void;
};

const ErrorContext = createContext<ErrorContextType | undefined>(undefined);

export const ErrorProvider: FC<{ children: ReactNode }> = ({ children }) => {
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const translateY = useSharedValue(200);

  const hideError = useCallback(() => {
    translateY.value = withTiming(200);
  }, []);

  const showError = useCallback(
    (message: string) => {
      setErrorMsg(message);
      translateY.value = withSpring(0, { damping: 15, stiffness: 120 });

      setTimeout(() => {
        hideError();
      }, 4000);
    },
    [hideError],
  );

  const animatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: translateY.value }],
  }));

  return (
    <ErrorContext.Provider value={{ showError }}>
      {children}

      <Animated.View
        style={animatedStyle}
        className="absolute bottom-0 left-0 right-0 z-[9999]"
      >
        <SafeAreaView edges={["bottom"]}>
          <Pressable
            onPress={hideError}
            className="bg-red-500 m-4 p-4 rounded-3xl shadow-xl shadow-black/40 flex-row items-center border border-red-400/20"
            style={{ elevation: 12 }}
          >
            <View className="bg-white/20 p-2 rounded-full mr-3">
              <MaterialCommunityIcons
                name="alert-circle-outline"
                size={24}
                color="white"
              />
            </View>

            <View className="flex-1">
              <Text className="text-white font-bold text-base">
                Wystąpił błąd
              </Text>
              <Text className="text-white/90 text-sm" numberOfLines={2}>
                {errorMsg || "Coś poszło nie tak..."}
              </Text>
            </View>

            <MaterialCommunityIcons
              name="close"
              size={20}
              color="white"
              style={{ opacity: 0.7 }}
            />
          </Pressable>
        </SafeAreaView>
      </Animated.View>
    </ErrorContext.Provider>
  );
};

export const useError = () => {
  const context = useContext(ErrorContext);
  if (!context) throw new Error("useError must be used within ErrorProvider");
  return context;
};
