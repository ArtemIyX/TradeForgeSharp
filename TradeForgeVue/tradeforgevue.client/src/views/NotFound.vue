<template>
  <v-container class="fill-height">
    <v-row align="center" justify="center">
      <v-col cols="12" sm="10" md="8" lg="6">
        <v-card class="pa-8 text-center" elevation="0">
          <!-- Animated 404 Number -->
          <div class="error-number mb-4">
            <span class="glitch text-primary" data-text="404">404</span>
          </div>

          <!-- Icon -->
          <v-icon
            size="120"
            color="primary"
            class="mb-6 bounce-animation"
          >
            mdi-emoticon-sad-outline
          </v-icon>

          <!-- Title -->
          <h1 class="text-h4 font-weight-bold mb-3">
            Oops! Page Not Found
          </h1>

          <!-- Description -->
          <p class="text-body-1 text-medium-emphasis mb-8">
            The page you're looking for doesn't exist or has been moved.
            Let's get you back on track!
          </p>

          <!-- Action Buttons -->
          <div class="d-flex flex-wrap justify-center gap-3">
            <v-btn
              color="primary"
              size="large"
              prepend-icon="mdi-home"
              @click="goHome"
              variant="flat"
              rounded="pill"
            >
              Go Home
            </v-btn>

            <v-btn
              color="secondary"
              size="large"
              prepend-icon="mdi-arrow-left"
              @click="goBack"
              variant="outlined"
              rounded="pill"
            >
              Go Back
            </v-btn>
          </div>

          <!-- Additional Help -->
          <v-divider class="my-8"></v-divider>

          <p class="text-caption text-medium-emphasis">
            Need help? Try these popular pages:
          </p>

          <div class="mt-4">
<!--            <v-chip
              class="ma-1"
              variant="outlined"
              @click="$router.push('/')"
              clickable
            >
              <v-icon start>mdi-view-dashboard</v-icon>
              Dashboard
            </v-chip>-->

            <v-chip
              class="ma-1"
              variant="outlined"
              @click="$router.push('/history')"
              clickable
            >
              <v-icon start>mdi-history</v-icon>
              History
            </v-chip>

            <v-chip
              class="ma-1"
              variant="outlined"
              @click="$router.push('/backtester')"
              clickable
            >
              <v-icon start>mdi-chart-line</v-icon>
              Backtester
            </v-chip>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import {useRouter} from 'vue-router'

const router = useRouter()

const goHome = () => {
  router.push('/')
}

const goBack = () => {
  router.back()
}
</script>

<style scoped>
.fill-height {
  min-height: 100vh;
}

.error-number {
  font-size: 120px;
  font-weight: 900;
  line-height: 1;

  opacity: 0.2;
}

.glitch {
  position: relative;
  display: inline-block;
  animation: glitch 2s infinite;
}

@keyframes glitch {
  0%, 100% {
    transform: translate(0);
  }
  20% {
    transform: translate(-2px, 2px);
  }
  40% {
    transform: translate(-2px, -2px);
  }
  60% {
    transform: translate(2px, 2px);
  }
  80% {
    transform: translate(2px, -2px);
  }
}

.bounce-animation {
  animation: bounce 2s ease-in-out infinite;
}

@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
}

.gap-3 {
  gap: 12px;
}

/* Hover effect for chips */
.v-chip:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease;
}
</style>
