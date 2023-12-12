CONTAINER_NAME ?= demo-ms-user.azurecr.io

IMAGE_NAME ?= demo-ms-user
DEPLOYMENT_NAME ?= demo-ms-user-deployment
SERVICE_NAME ?= demo-ms-user-service

DEPLOYMENT_FILE ?= deployment.yml
SERVICE_FILE ?= service.yml
INGRESS_FILE ?= ingress.yml
ALL_FILE ?= all.yaml

info:
	@external_ip=$$(kubectl get services | grep ${SERVICE_NAME} | awk '{print $$4}'); \
	echo "The external IP is: $${external_ip}"

docker-build:
	docker build --tag ${CONTAINER_NAME}/${IMAGE_NAME} -f Dockerfile ..

docker-push:
	docker push ${CONTAINER_NAME}/${IMAGE_NAME}

docker-patch: docker-build docker-push

kubectl-apply:
	kubectl --insecure-skip-tls-verify apply -f ${DEPLOYMENT_FILE} && \
    kubectl --insecure-skip-tls-verify apply -f ${SERVICE_FILE} && \
    kubectl --insecure-skip-tls-verify apply -f ${INGRESS_FILE}

kubectl-delete:
	kubectl --insecure-skip-tls-verify delete -f ${DEPLOYMENT_FILE} && \
	kubectl --insecure-skip-tls-verify delete -f ${SERVICE_FILE} && \
	kubectl --insecure-skip-tls-verify delete -f ${INGRESS_FILE}

kubectl-reset: kubectl-delete kubectl-apply

all: docker-patch kubectl-apply

reset: kubectl-delete all

kube-copy:
	sudo cp -r /mnt/c/Users/khomcvla/.kube/config ~/.kube/config

kube-delete:
	sudo rm -r ~/.kube/config

kube-patch:
	kube-delete kube-copy

kubectl-apply-ingress:
	sudo kubectl --insecure-skip-tls-verify apply -f "https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml"
