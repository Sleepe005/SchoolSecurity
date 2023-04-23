import cv2 as cv
import time

# Настройки YOLO
Conf_threshold = 0.4
NMS_threshold = 0.4
COLORS = [(0, 255, 0), (0, 0, 255), (255, 0, 0),
          (255, 255, 0), (255, 0, 255), (0, 255, 255)]

# Считываем список возможных класов
class_name = []
with open('classes.txt', 'r') as f:
    class_name = [cname.strip() for cname in f.readlines()]

# Задаём веса и конфигурации модели YOLO
net = cv.dnn.readNet('yolo.weights', 'yolo.cfg')
# Используем GPU
net.setPreferableBackend(cv.dnn.DNN_BACKEND_CUDA)
net.setPreferableTarget(cv.dnn.DNN_TARGET_CUDA_FP16)

model = cv.dnn_DetectionModel(net)
model.setInputParams(size=(416, 416), scale=1/255, swapRB=True)

# Получаем изображение с камеры
cap = cv.VideoCapture(0)
starting_time = time.time()
frame_counter = 0

while True:
    ret, frame = cap.read()
    frame_counter += 1
    if ret == False:
        break

    # На каждом кадре запускаем работу модели
    classes, scores, boxes = model.detect(frame, Conf_threshold, NMS_threshold)

    for (classid, score, box) in zip(classes, scores, boxes):
        # Если обнаружили объект инициализируем его класс и положение
        color = COLORS[int(classid) % len(COLORS)]
        label = "%s : %f" % (class_name[classid], score)
        # Выводим инфрмацию на экран
        cv.rectangle(frame, box, color, 1)
        cv.putText(frame, label,  (box[0], box[1]-10),
                   cv.FONT_HERSHEY_COMPLEX, 0.3, color, 1)
        
    # Высчитываем FPS
    endingTime = time.time() - starting_time
    fps = frame_counter/endingTime
    cv.putText(frame, f'FPS: {fps}', (20, 50),
               cv.FONT_HERSHEY_COMPLEX, 0.7, (0, 255, 0), 2)
    
    cv.imshow('frame', frame)
    key = cv.waitKey(1)

    if key == ord('q'):
        break
    
cap.release()
cv.destroyAllWindows()