import { html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context.ts";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import "./seed-response.element.ts";
import "./seed-mode-custom.element.ts";
import "./seed-mode-full.element.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { SeedResponse } from "../../generated/index.ts";

@customElement("enterspeed-seed-modes")
export class enterspeedSeedModesElement extends UmbLitElement {
  #enterspeedContext!: EnterspeedContext;
  #notificationContext!: UmbNotificationContext;
  #numberOfPendingJobs = 0;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: String })
  selectedSeedMode?: string;

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  @property({ type: Boolean })
  disableSeedButton?: boolean;

  constructor() {
    super();
    this.#initGetNumberOfPendingJobs();

    this.#enterspeedContext = new EnterspeedContext(this);
    this.consumeContext(
      UMB_NOTIFICATION_CONTEXT,
      (instance: UmbNotificationContext) => {
        this.#notificationContext = instance;
      }
    );
  }

  async #seed() {
    this.disableSeedButton = true;
    this.#enterspeedContext!.seed()
      .then((response) => {
        if (response.data?.isSuccess) {
          this.seedResponse = response.data.data;

          this.dispatchEvent(
            new CustomEvent("seed-response", {
              bubbles: true,
              detail: this.seedResponse,
            })
          );

          this.#notificationContext?.peek("positive", {
            data: {
              headline: "Seed",
              message: "Successfully started seeding to Enterspeed",
            },
          });

          this.numberOfPendingJobs =
            this.seedResponse?.numberOfPendingJobs || 0;
        } else {
          this.seedResponse = null;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: error.data.message,
          },
        });
      });

    this.disableSeedButton = false;
  }

  async #clearJobQueue() {
    this.#enterspeedContext!.clearJobQueue()
      .then((response) => {
        if (response.data?.isSuccess) {
          this.#notificationContext?.peek("positive", {
            data: {
              headline: "Clear job queue",
              message: "Successfully cleared the queue of pending jobs",
            },
          });
          this.numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Clear job queue",
            message: error.data.message,
          },
        });
      });

    this.seedResponse = null;
  }

  #initGetNumberOfPendingJobs() {
    let intervalId = setInterval(
      () => this.#getNumberOfPendingJobs(),
      10 * 1000
    );
    window.addEventListener(
      "hashchange",
      () => {
        clearInterval(intervalId);
      },
      false
    );
  }

  #getNumberOfPendingJobs() {
    this.#enterspeedContext
      .getNumberOfPendingJobs()
      .then((response) => {
        if (response.data?.isSuccess) {
          this.#numberOfPendingJobs =
            response.data?.data?.numberOfPendingJobs ?? 0;
          if (this.#numberOfPendingJobs === 0) {
            this.seedResponse = null;
          }
        } else {
          this.#numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Failed to check queue length",
            message: error.data.message,
          },
        });
      });
  }

  render() {
    return html` ${this.#renderSeedModes()}
      <enterspeed-seed-response
        .seedResponse=${this.seedResponse}
      ></enterspeed-seed-response>`;
  }

  #renderSeedModes() {
    return html`${this.selectedSeedMode == "Everything"
      ? this.#renderFullSeed()
      : this.#renderCustomSeed()}
    ${this.#renderButtons()}`;
  }

  #renderFullSeed() {
    return html`<enterspeed-seed-mode-full
      .seedResponse=${this.seedResponse}
      .numberOfPendingJobs=${this.#numberOfPendingJobs}
      @seed-response=${(e: CustomEvent) => {
        this.seedResponse = e.detail;
        this.requestUpdate();
      }}
    ></enterspeed-seed-mode-full>`;
  }

  #renderCustomSeed() {
    return html`<enterspeed-seed-mode-custom
      .seedResponse=${this.seedResponse}
      .numberOfPendingJobs=${this.#numberOfPendingJobs}
    ></enterspeed-seed-mode-custom>`;
  }



  
  #renderButtons() {
    return html` <div class="seed-dashboard-content">
      ${this.#renderSeedButton()} ${this.#renderClearJobQueueButton()}
    </div>`;
  }

  #renderSeedButton() {
    if (this.disableSeedButton) {
      return html`<uui-button
        .disabled=${this.disableSeedButton}
        type="button"
        look="primary"
        color="default"
        label="Seed"
        @click=${() => this.#seed()}
      ></uui-button>`;
    }
  }

  #renderClearJobQueueButton() {
    return html` <uui-button
      .disabled=${this.numberOfPendingJobs > 0}
      type="button"
      look="secondary"
      color="default"
      label="Clear job queue ${this.numberOfPendingJobs}"
      @click="${async () => this.#clearJobQueue()}"
    ></uui-button>`;
  }
}

export default enterspeedSeedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed-modes": enterspeedSeedModesElement;
  }
}
