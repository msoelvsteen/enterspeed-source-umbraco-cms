import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { EnterspeedRepository } from "./repository/enterspeed.repository";
import {
  CustomSeedModel,
  EnterspeedUmbracoConfiguration,
  JobIdsToDelete,
} from "./generated";
import { customNodesSelected } from "./types";


export class EnterspeedContext extends UmbContextBase<EnterspeedContext> {
  protected enterspeedRepository = new EnterspeedRepository(this);

  constructor(host: UmbControllerHost) {
    super(host, ENTERSPEED_CONTEXT);
  }

  public async customSeed(customSeedModel: CustomSeedModel) {
    return await this.enterspeedRepository.customSeed(customSeedModel);
  }

  public async seed(nodesSelected: customNodesSelected) {
    let response = await this.enterspeedRepository.seed();
    return response;
  }

  public async clearJobQueue() {
    return await this.enterspeedRepository.clearPendingJobs();
  }

  public async getNumberOfPendingJobs() {
    return await this.enterspeedRepository.getNumberOfPendingJobs();
  }

  public async getEnterspeedConfiguration() {
    return await this.enterspeedRepository.getEnterspeedConfiguration();
  }

  public async saveConfiguration(
    configuration: EnterspeedUmbracoConfiguration
  ) {
    return await this.enterspeedRepository.saveConfiguration(configuration);
  }

  public async testConfigurationConnection(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return await this.enterspeedRepository.testConfigurationConnection(
      enterspeedUmbracoConfiguration
    );
  }

  public async getFailedJobs() {
    let response = await this.enterspeedRepository.getFailedJobs();
    return response;
  }

  public async deleteSelectedFailedJobs(ids: JobIdsToDelete) {
    return await this.enterspeedRepository.deleteSelectedFailedJobs(ids);
  }

  public async deleteFailedJobs() {
    return await this.enterspeedRepository.deleteFailedJobs();
  }
}

export const ENTERSPEED_CONTEXT = new UmbContextToken<EnterspeedContext>(
  EnterspeedContext.name
);
