FROM mono:latest as builder

WORKDIR /build

# Install build dependencies
RUN apt-get update \
    && apt-get install -yq wget \
    && wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get install -yq apt-transport-https \
    && apt-get update \
    && apt-get install -yq dotnet-sdk-5.0

# Copy in source files
COPY . .

# Compile project
RUN msbuild -m MCGalaxy.sln /property:Configuration=Release

FROM mono:latest

# Add user for application and create source directories
RUN groupadd -g 1000 galaxy && \
    useradd -u 1000 -ms /bin/bash -g galaxy galaxy

ENV MCGALAXY /MCGalaxy
ENV DATA_DIR ${MCGALAXY}/data
WORKDIR ${MCGALAXY}

COPY docker-entrypoint.sh /
COPY --from=builder --chown=galaxy:galaxy /build/bin/Release/ ./

RUN chmod +x /docker-entrypoint.sh \
    && mkdir -p ${DATA_DIR} \
    && chown -R galaxy:galaxy ${DATA_DIR} \
    && chown -R galaxy:galaxy ${MCGALAXY}

USER galaxy

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["mono", "--gc=sgen", "MCGalaxyCLI.exe" ]

EXPOSE 25565
VOLUME ${DATA_DIR}
