/**
 * plugins/index.ts
 *
 * Automatically included in `./src/main.ts`
 */

// Plugins
import { loadFonts } from './webfontloader'
import vuetify from './vuetify'
import pinia from '../store'
import router from '../router'

// Types
import type { App } from 'vue'
import VuetifyUseDialog from 'vuetify-use-dialog'

export function registerPlugins(app: App) {
  loadFonts()
  app
    .use(vuetify)
    .use(router)
    .use(pinia)
    .use(VuetifyUseDialog, {
      confirmDialog: {
        dialogProps: {
          'max-width': '500px'
        },
        cardProps: { color: 'grey-darken-4' },
        cardTitleProps: { class: 'text-h6' },
      },
      snackbar: {
        snackbarProps: {
          absolute: true,
          timeout: 3000,
          zIndex: 99999
        },
        showCloseButton: false
      },
      
    })
}
